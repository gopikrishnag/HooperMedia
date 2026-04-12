import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

interface LanguageOption {
  code: string;
  label: string;
}

interface LanguageConfig {
  defaultLanguage: string;
  languages: LanguageOption[];
  translationFiles?: Record<string, string>;
}

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly platformId = inject(PLATFORM_ID);

  private readonly fallbackConfig: LanguageConfig = {
    defaultLanguage: 'en',
    languages: [
      { code: 'en', label: 'English' },
      { code: 'fr', label: 'Franch' },
      { code: 'es', label: 'Spanish' },
      { code: 'de', label: 'German' }
    ]
  };

  private config: LanguageConfig = this.fallbackConfig;
  
  // NOTE: Fallback translations to ensure basic functionality even if config or translation files fail to load.
  private readonly fallbackTranslations: Record<string, unknown> = {
    app: {
      menu: {
        openMain: 'Open main menu',
        person: 'Person',
        address: 'Address',
        viewNotifications: 'View notifications'
      },
      language: {
        label: 'Language'
      }
    },
    person: {
      title: 'Persons',
      form: {
        nameLabel: 'Name',
        namePlaceholder: 'Enter full name',
        dateOfBirthLabel: 'Date of Birth'
      },
      table: {
        name: 'Name',
        dateOfBirth: 'Date of Birth',
        actions: 'Actions'
      },
      actions: {
        add: 'Add Person',
        update: 'Update Person',
        cancel: 'Cancel',
        edit: 'Edit',
        delete: 'Delete'
      },
      validation: {
        nameRequired: 'Name is required.',
        nameMaxLength: 'Name must not exceed 50 characters.',
        dateOfBirthRequired: 'Date of birth is required.',
        futureDate: 'Date of birth cannot be in the future.',
        tooOld: 'Age cannot exceed 100 years.'
      },
      messages: {
        loading: 'Loading persons...',
        noData: 'No persons found.',
        loadFailed: 'Failed to load persons.',
        createFailed: 'Failed to create person.',
        updateFailed: 'Failed to update person.',
        deleteFailed: 'Failed to delete person.',
        deleteConfirm: 'Delete {name}?'
      }
    },
    address: {
      title: 'Addresses',
      form: {
        personIdLabel: 'Person Id',
        personIdPlaceholder: 'Select person id',
        addressLine1Label: 'Address Line 1',
        addressLine1Placeholder: 'Enter address line 1',
        addressLine2Label: 'Address Line 2',
        addressLine2Placeholder: 'Enter address line 2',
        townOrCityLabel: 'Town or City',
        townOrCityPlaceholder: 'Enter town or city',
        zipOrPostCodeLabel: 'Zip or Post Code',
        zipOrPostCodePlaceholder: 'Enter zip or post code',
        countryLabel: 'Country',
        countryPlaceholder: 'Enter country'
      },
      table: {
        personId: 'Person Id',
        addressLine1: 'Address Line 1',
        addressLine2: 'Address Line 2',
        townOrCity: 'Town or City',
        zipOrPostCode: 'Zip or Post Code',
        country: 'Country',
        actions: 'Actions'
      },
      actions: {
        add: 'Add Address',
        update: 'Update Address',
        cancel: 'Cancel',
        edit: 'Edit',
        delete: 'Delete'
      },
      validation: {
        personIdRequired: 'Person selection is required.',
        addressLine1Required: 'Address Line 1 is required.',
        addressLine1MaxLength: 'Address Line 1 must not exceed 100 characters.',
        addressLine2MaxLength: 'Address Line 2 must not exceed 100 characters.',
        townOrCityRequired: 'Town or City is required.',
        townOrCityMaxLength: 'Town or City must not exceed 100 characters.',
        zipOrPostCodeRequired: 'Zip or Post Code is required.',
        zipOrPostCodeMaxLength: 'Zip or Post Code must not exceed 15 characters.',
        countryRequired: 'Country is required.',
        countryMaxLength: 'Country must not exceed 50 characters.'
      },
      messages: {
        loadingPersons: 'Loading persons...',
        loading: 'Loading addresses...',
        selectPerson: 'Select a person to view addresses.',
        noPersons: 'No persons available. Create a person first.',
        noData: 'No addresses found for the selected person.',
        loadPersonsFailed: 'Failed to load persons.',
        loadFailed: 'Failed to load addresses.',
        createFailed: 'Failed to create address.',
        updateFailed: 'Failed to update address.',
        deleteFailed: 'Failed to delete address.',
        deleteConfirm: 'Delete address "{addressLine1}"?'
      }
    }
  };

  readonly languages = signal<LanguageOption[]>(this.fallbackConfig.languages);
  readonly currentLanguage = signal<string>(this.fallbackConfig.defaultLanguage);

  private readonly translations = signal<Record<string, unknown>>(this.fallbackTranslations);

  constructor(private readonly httpClient: HttpClient) {}

  async init(): Promise<void> {
    if (!isPlatformBrowser(this.platformId)) {
      this.languages.set(this.fallbackConfig.languages);
      this.currentLanguage.set(this.fallbackConfig.defaultLanguage);
      this.translations.set(this.fallbackTranslations);
      return;
    }

    await this.loadConfig();
    await this.setLanguage(this.config.defaultLanguage);
  }

  async setLanguage(languageCode: string): Promise<void> {
    const translationPath =
      this.config.translationFiles?.[languageCode] ?? `/i18n/${languageCode}.json`;

    try {
      const data = await firstValueFrom(
        this.httpClient.get<Record<string, unknown>>(translationPath)
      );

      this.translations.set(data);
      this.currentLanguage.set(languageCode);
    } catch {
      if (languageCode !== this.config.defaultLanguage) {
        await this.setLanguage(this.config.defaultLanguage);
      }
    }
  }

  translate(key: string, params?: Record<string, string | number>): string {
    const value = this.getByPath(this.translations(), key);
    const template = typeof value === 'string' ? value : key;

    if (!params) {
      return template;
    }

    return template.replace(/\{(\w+)\}/g, (_, token: string) => {
      return String(params[token] ?? `{${token}}`);
    });
  }

  private async loadConfig(): Promise<void> {
    try {
      const config = await firstValueFrom(
        this.httpClient.get<LanguageConfig>('/i18n/config.json')
      );

      this.config = config;
      this.languages.set(config.languages);
    } catch {
      this.config = this.fallbackConfig;
      this.languages.set(this.fallbackConfig.languages);
    }
  }

  private getByPath(source: Record<string, unknown>, path: string): unknown {
    return path.split('.').reduce<unknown>((acc, key) => {
      if (acc && typeof acc === 'object' && key in (acc as Record<string, unknown>)) {
        return (acc as Record<string, unknown>)[key];
      }

      return undefined;
    }, source);
  }
}
