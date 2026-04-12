import { Routes } from '@angular/router';
import { AddressComponent } from './features/address/address.component';
import { PersonComponent } from './features/person/person.component';

export const routes: Routes = [
	{
		path: '',
		pathMatch: 'full',
		redirectTo: 'persons'
	},
	{
		path: 'persons',
		component: PersonComponent
	},
	{
		path: 'addresses',
		component: AddressComponent
	},
	{
		path: '**',
		redirectTo: 'persons'
	}
];
