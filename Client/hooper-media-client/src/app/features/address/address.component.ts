import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, PLATFORM_ID, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { distinctUntilChanged } from 'rxjs/operators';

import { LanguageService } from '../../core/services/language.service';
import { PersonService } from '../person/person.service';
import { Person } from '../person/person.model';
import { ADDRESS_VALIDATION } from './config/address-validation.config';
import { Address, CreateAddressRequest } from './address.model';
import { AddressService } from './address.service';

@Component({
  selector: 'app-address',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './address.component.html'
})
export class AddressComponent implements OnInit, OnDestroy {
  private readonly addressService = inject(AddressService);
  private readonly personService = inject(PersonService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly languageService = inject(LanguageService);
  private isDestroyed = false;

  readonly addressForm = this.formBuilder.group({
    personId: [null as number | null, [Validators.required]],
    addressLine1: ['', [Validators.required, Validators.maxLength(ADDRESS_VALIDATION.addressLineMaxLength)]],
    addressLine2: ['', [Validators.maxLength(ADDRESS_VALIDATION.addressLineMaxLength)]],
    townOrCity: ['', [Validators.required, Validators.maxLength(ADDRESS_VALIDATION.addressLineMaxLength)]],
    zipOrPostCode: ['', [Validators.required, Validators.maxLength(ADDRESS_VALIDATION.zipOrPostCodeMaxLength)]],
    country: ['', [Validators.required, Validators.maxLength(ADDRESS_VALIDATION.countryMaxLength)]]
  });

  readonly persons = signal<Person[]>([]);
  readonly addresses = signal<Address[]>([]);
  readonly loadingPersons = signal(false);
  readonly loadingAddresses = signal(false);
  readonly submitting = signal(false);
  readonly errorMessage = signal('');
  readonly selectedPersonId = signal<number | null>(null);
  readonly editingAddressId = signal<number | null>(null);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadPersons();
      this.setupPersonIdChangeListener();
    }
  }

  private setupPersonIdChangeListener(): void {
    this.addressForm
      .get('personId')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((personId: number | null) => {
        if (this.isDestroyed) {
          return;
        }

        this.addresses.set([]);

        if (personId && Number.isInteger(personId) && personId > 0) {
          this.selectedPersonId.set(personId);
          this.cancelEdit();
          this.loadAddresses(personId);
        } else {
          this.selectedPersonId.set(null);
          this.cancelEdit();
        }
      });
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
  }

  get submitButtonText(): string {
    return this.editingAddressId() === null
      ? this.translate('address.actions.add')
      : this.translate('address.actions.update');
  }

  get isEditMode(): boolean {
    return this.editingAddressId() !== null;
  }

  trackByAddressId(_index: number, address: Address): number {
    return address.addressId;
  }

  loadPersons(): void {
    this.loadingPersons.set(true);
    this.errorMessage.set('');

    this.personService.list().subscribe({
      next: (persons) => {
        this.persons.set(persons);
        this.loadingPersons.set(false);

        if (persons.length === 0) {
          this.selectedPersonId.set(null);
          this.addresses.set([]);
          return;
        }

        const initialPersonId = persons[0].personId;
        this.selectedPersonId.set(initialPersonId);
        this.addressForm.patchValue({ personId: initialPersonId });
        this.loadAddresses(initialPersonId);
      },
      error: () => {
        this.errorMessage.set(this.translate('address.messages.loadPersonsFailed'));
        this.loadingPersons.set(false);
      }
    });
  }

  loadAddresses(personId: number): void {
    this.loadingAddresses.set(true);
    this.errorMessage.set('');

    this.addressService.listByPersonId(personId).subscribe({
      next: (addresses) => {
        this.addresses.set(addresses);
        this.loadingAddresses.set(false);
      },
      error: () => {
        this.errorMessage.set(this.translate('address.messages.loadFailed'));
        this.loadingAddresses.set(false);
      }
    });
  }

  saveAddress(): void {
    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set('');

    const payload = this.buildRequestPayload();

    if (this.editingAddressId() === null) {
      this.addressService.create(payload).subscribe({
        next: () => {
          this.afterSuccessfulSave();
        },
        error: () => {
          this.handleSaveError(this.translate('address.messages.createFailed'));
        }
      });
      return;
    }

    this.addressService.update(this.editingAddressId()!, payload).subscribe({
      next: () => {
        this.afterSuccessfulSave();
      },
      error: () => {
        this.handleSaveError(this.translate('address.messages.updateFailed'));
      }
    });
  }

  editAddress(address: Address): void {
    this.editingAddressId.set(address.addressId);
    this.errorMessage.set('');
    this.selectedPersonId.set(address.personId);
    this.addressForm.setValue({
      personId: address.personId,
      addressLine1: address.addressLine1,
      addressLine2: address.addressLine2 ?? '',
      townOrCity: address.townOrCity,
      zipOrPostCode: address.zipOrPostCode,
      country: address.country
    });
  }

  cancelEdit(): void {
    this.editingAddressId.set(null);
    this.addressForm.reset({
      personId: this.selectedPersonId(),
      addressLine1: '',
      addressLine2: '',
      townOrCity: '',
      zipOrPostCode: '',
      country: ''
    });
  }

  deleteAddress(address: Address): void {
    const shouldDelete = window.confirm(
      this.translate('address.messages.deleteConfirm', { addressLine1: address.addressLine1 })
    );

    if (!shouldDelete) {
      return;
    }

    this.errorMessage.set('');
    this.addressService.delete(address.addressId).subscribe({
      next: () => {
        if (this.editingAddressId() === address.addressId) {
          this.cancelEdit();
        }

        const personId = this.selectedPersonId();
        if (personId !== null) {
          this.loadAddresses(personId);
        }
      },
      error: () => {
        this.errorMessage.set(this.translate('address.messages.deleteFailed'));
      }
    });
  }

  translate(key: string, params?: Record<string, string | number>): string {
    return this.languageService.translate(key, params);
  }

  private buildRequestPayload(): CreateAddressRequest {
    const formValue = this.addressForm.getRawValue();

    return {
      personId: formValue.personId!,
      addressLine1: formValue.addressLine1!.trim(),
      addressLine2: formValue.addressLine2?.trim() || null,
      townOrCity: formValue.townOrCity!.trim(),
      zipOrPostCode: formValue.zipOrPostCode!.trim(),
      country: formValue.country!.trim()
    };
  }

  private afterSuccessfulSave(): void {
    this.submitting.set(false);
    this.cancelEdit();

    const personId = this.selectedPersonId();
    if (personId !== null) {
      this.loadAddresses(personId);
    }
  }

  private handleSaveError(message: string): void {
    this.errorMessage.set(message);
    this.submitting.set(false);
  }
}
