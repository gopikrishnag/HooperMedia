import { PLATFORM_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { AddressComponent } from './address.component';
import { AddressService } from './address.service';
import { PersonService } from '../person/person.service';
import { LanguageService } from '../../core/services/language.service';
import { Address } from './address.model';
import { Person } from '../person/person.model';

describe('AddressComponent', () => {
  let fixture: ComponentFixture<AddressComponent>;
  let component: AddressComponent;

  const persons: Person[] = [
    { personId: 1, name: 'Alice', dateOfBirth: '1990-01-01' },
    { personId: 2, name: 'Bob', dateOfBirth: '1988-05-10' }
  ];

  const addressesByPerson: Record<number, Address[]> = {
    1: [
      {
        addressId: 11,
        personId: 1,
        addressLine1: '10 Main St',
        addressLine2: null,
        townOrCity: 'London',
        zipOrPostCode: 'E1 1AA',
        country: 'UK'
      }
    ],
    2: [
      {
        addressId: 22,
        personId: 2,
        addressLine1: '21 Park Ave',
        addressLine2: 'Unit 2',
        townOrCity: 'Paris',
        zipOrPostCode: '75001',
        country: 'France'
      }
    ]
  };

  const addressServiceMock = {
    listByPersonId: vi.fn((personId: number) => of(addressesByPerson[personId] ?? [])),
    create: vi.fn(() => of({})),
    update: vi.fn(() => of({})),
    delete: vi.fn(() => of(void 0))
  };

  const personServiceMock = {
    list: vi.fn(() => of(persons))
  };

  const languageServiceMock = {
    translate: vi.fn((key: string) => key)
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddressComponent],
      providers: [
        { provide: AddressService, useValue: addressServiceMock },
        { provide: PersonService, useValue: personServiceMock },
        { provide: LanguageService, useValue: languageServiceMock },
        { provide: PLATFORM_ID, useValue: 'browser' }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddressComponent);
    component = fixture.componentInstance;

    addressServiceMock.listByPersonId.mockClear();
    addressServiceMock.create.mockClear();
    addressServiceMock.update.mockClear();
    addressServiceMock.delete.mockClear();
    personServiceMock.list.mockClear();
  });

  it('loads persons and initial person addresses on init', () => {
    fixture.detectChanges();

    expect(personServiceMock.list).toHaveBeenCalled();
    expect(component.selectedPersonId()).toBe(1);
    expect(addressServiceMock.listByPersonId).toHaveBeenCalledWith(1);
    expect(component.addresses()).toEqual(addressesByPerson[1]);
  });

  it('reloads addresses when selected person changes', () => {
    fixture.detectChanges();
    addressServiceMock.listByPersonId.mockClear();

    component.addressForm.controls.personId.setValue(2);

    expect(component.selectedPersonId()).toBe(2);
    expect(addressServiceMock.listByPersonId).toHaveBeenCalledWith(2);
    expect(component.addresses()).toEqual(addressesByPerson[2]);
  });

  it('does not submit when form is invalid', () => {
    fixture.detectChanges();

    component.addressForm.patchValue({
      personId: 1,
      addressLine1: '',
      townOrCity: '',
      zipOrPostCode: '',
      country: ''
    });

    component.saveAddress();

    expect(addressServiceMock.create).not.toHaveBeenCalled();
    expect(component.addressForm.controls.addressLine1.touched).toBe(true);
  });

  it('creates address with trimmed payload when form is valid', () => {
    fixture.detectChanges();

    component.addressForm.patchValue({
      personId: 2,
      addressLine1: '  5 High Street  ',
      addressLine2: '   ',
      townOrCity: '  Eastham ',
      zipOrPostCode: ' E12 3HS ',
      country: ' UK '
    });

    expect(component.addressForm.controls.personId.value).toBe(2);

    component.saveAddress();

    expect(addressServiceMock.create).toHaveBeenCalledWith({
      personId: 2,
      addressLine1: '5 High Street',
      addressLine2: null,
      townOrCity: 'Eastham',
      zipOrPostCode: 'E12 3HS',
      country: 'UK'
    });
  });
});
