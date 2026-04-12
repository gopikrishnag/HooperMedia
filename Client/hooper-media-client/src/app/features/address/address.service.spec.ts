import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { AddressService } from './address.service';
import { API_BASE_URL } from '../../core/config/api.config';

describe('AddressService', () => {
  let service: AddressService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AddressService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'http://localhost:8080' }
      ]
    });

    service = TestBed.inject(AddressService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('calls listByPersonId endpoint for selected person', () => {
    const personId = 4;

    service.listByPersonId(personId).subscribe();

    const request = httpTestingController.expectOne('http://localhost:8080/address/person/4');
    expect(request.request.method).toBe('GET');
    request.flush([]);
  });
});
