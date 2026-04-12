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
      { code: 'es', label: 'Spanish' }
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
        dateOfBirthRequired: 'Date of birth is required.'
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

  t(key: string, params?: Record<string, string | number>): string {
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
