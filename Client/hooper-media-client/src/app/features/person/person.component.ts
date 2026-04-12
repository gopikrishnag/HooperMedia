
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, PLATFORM_ID, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Person, CreatePersonRequest } from './person.model';
import { PersonService } from './person.service';
import { LanguageService } from '../../core/services/language.service';
import { PERSON_VALIDATION } from './config/person-validation.config';
import { dateOfBirthValidator, nameValidator } from './validators/person.validators';
@Component({
  selector: 'app-person',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './person.component.html'
})
export class PersonComponent {
  private readonly personService = inject(PersonService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly languageService = inject(LanguageService);

  readonly personForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(PERSON_VALIDATION.nameMaxLength), nameValidator]],
    dateOfBirth: ['', [Validators.required, dateOfBirthValidator]]
  });

  readonly persons = signal<Person[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly errorMessage = signal('');
  readonly editingPersonId = signal<number | null>(null);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadPersons();
    }
  }

  get submitButtonText(): string {
    return this.editingPersonId() === null
      ? this.translate('person.actions.add')
      : this.translate('person.actions.update');
  }

  get isEditMode(): boolean {
    return this.editingPersonId() !== null;
  }

  trackByPersonId(_index: number, person: Person): number {
    return person.personId;
  }

  loadPersons(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.personService.list().subscribe({
      next: (persons) => {
        //console.log('Loaded persons:', persons);
        this.persons.set(persons);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set(this.translate('person.messages.loadFailed'));
        this.loading.set(false);
      }
    });
  }

  savePerson(): void {
    if (this.personForm.invalid) {
      this.personForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set('');

    const payload = this.buildRequestPayload();

    if (this.editingPersonId() === null) {
      this.personService.create(payload).subscribe({
        next: () => {
          this.afterSuccessfulSave();
        },
        error: () => {
          this.handleSaveError(this.translate('person.messages.createFailed'));
        }
      });
      return;
    }

    this.personService.update(this.editingPersonId()!, payload).subscribe({
      next: () => {
        this.afterSuccessfulSave();
      },
      error: () => {
        this.handleSaveError(this.translate('person.messages.updateFailed'));
      }
    });
  }

  editPerson(person: Person): void {
    this.editingPersonId.set(person.personId);
    this.errorMessage.set('');
    this.personForm.setValue({
      name: person.name,
      dateOfBirth: this.toInputDate(person.dateOfBirth)
    });
  }

  cancelEdit(): void {
    this.editingPersonId.set(null);
    this.personForm.reset({
      name: '',
      dateOfBirth: ''
    });
  }

  deletePerson(person: Person): void {
    const shouldDelete = window.confirm(this.translate('person.messages.deleteConfirm', { name: person.name }));
    if (!shouldDelete) {
      return;
    }

    this.errorMessage.set('');
    this.personService.delete(person.personId).subscribe({
      next: () => {
        if (this.editingPersonId() === person.personId) {
          this.cancelEdit();
        }
        this.loadPersons();
      },
      error: () => {
        this.errorMessage.set(this.translate('person.messages.deleteFailed'));
      }
    });
  }

  translate(key: string, params?: Record<string, string | number>): string {
    return this.languageService.translate(key, params);
  }

  private buildRequestPayload(): CreatePersonRequest {
    const formValue = this.personForm.getRawValue();
    return {
      name: formValue.name.trim(),
      dateOfBirth: `${formValue.dateOfBirth}T00:00:00`
    };
  }

  private afterSuccessfulSave(): void {
    this.submitting.set(false);
    this.cancelEdit();
    this.loadPersons();
  }

  private handleSaveError(message: string): void {
    this.errorMessage.set(message);
    this.submitting.set(false);
  }

  private toInputDate(value: string): string {
    return value.slice(0, 10);
  }
}