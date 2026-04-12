import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, PLATFORM_ID, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Person, CreatePersonRequest } from './person.model';
import { PersonService } from './person.service';

@Component({
  selector: 'app-person',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './person.component.html'
})
export class PersonComponent {
  private readonly personService = inject(PersonService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly platformId = inject(PLATFORM_ID);

  readonly personForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    dateOfBirth: ['', [Validators.required]]
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

  get submitButtonLabel(): string {
    return this.editingPersonId() === null ? 'Add Person' : 'Update Person';
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
        this.errorMessage.set('Failed to load persons.');
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
          this.handleSaveError('Failed to create person.');
        }
      });
      return;
    }

    this.personService.update(this.editingPersonId()!, payload).subscribe({
      next: () => {
        this.afterSuccessfulSave();
      },
      error: () => {
        this.handleSaveError('Failed to update person.');
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
    const shouldDelete = window.confirm(`Delete ${person.name}?`);
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
        this.errorMessage.set('Failed to delete person.');
      }
    });
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