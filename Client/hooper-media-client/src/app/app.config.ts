import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { API_BASE_URL } from './core/config/api.config';

//TODO: move API_BASE_URL to environment variable and use different values for development and production
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideClientHydration(withEventReplay()),
    {
      provide: API_BASE_URL,
      useValue: 'http://localhost:8080' // Docker
      //useValue: 'http://localhost:5005' // Local development
    }
  ]
};
