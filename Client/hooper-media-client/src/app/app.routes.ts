import { Routes } from '@angular/router';
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
		path: '**',
		redirectTo: 'persons'
	}
];
