import { Injectable } from '@angular/core';

import { CrudApiService } from '../../core/services/crud-api.service';
import { CreatePersonRequest, Person, UpdatePersonRequest } from './person.model';

@Injectable({ providedIn: 'root' })
export class PersonService extends CrudApiService<Person, CreatePersonRequest, UpdatePersonRequest, number> {
  protected override readonly resourcePath = 'person';
}