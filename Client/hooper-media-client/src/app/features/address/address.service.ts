import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { CrudApiService } from '../../core/services/crud-api.service';
import { Address, CreateAddressRequest, UpdateAddressRequest } from './address.model';

@Injectable({ providedIn: 'root' })
export class AddressService extends CrudApiService<Address, CreateAddressRequest, UpdateAddressRequest, number> {
  protected override readonly resourcePath = 'address';

  listByPersonId(personId: number): Observable<Address[]> {
    return this.http.get<Address[]>(`${this.resourceUrl}/person/${personId}`);
  }
}
