import { AbstractControl, ValidationErrors } from '@angular/forms';

import { PERSON_VALIDATION } from '../config/person-validation.config';

export function nameValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const name = String(control.value).trim();
  if (name.length > PERSON_VALIDATION.nameMaxLength) {
    return {
      nameMaxLength: {
        max: PERSON_VALIDATION.nameMaxLength,
        actual: name.length
      }
    };
  }

  return null;
}

export function dateOfBirthValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const selectedDate = new Date(control.value);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (selectedDate > today) {
    return { futureDate: true };
  }

  const oldestAllowedDate = new Date();
  oldestAllowedDate.setFullYear(oldestAllowedDate.getFullYear() - PERSON_VALIDATION.maxAgeYears);

  if (selectedDate < oldestAllowedDate) {
    return { tooOld: true };
  }

  return null;
}
