import { APP_INITIALIZER, ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { API_BASE_URL } from './core/config/api.config';
import { LanguageService } from './core/services/language.service';

//TODO: move API_BASE_URL to environment variable and use different values for development and production
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withPreloading(PreloadAllModules)),
    provideHttpClient(),
    provideClientHydration(withEventReplay()),
    {
      provide: API_BASE_URL,
      useValue: 'http://localhost:8080' // Docker
      //useValue: 'http://localhost:5005' // Local development
    },
    {
      provide: APP_INITIALIZER,
      multi: true,
      deps: [LanguageService],
      useFactory: (languageService: LanguageService) => () => languageService.init()
    }
  ]
};
