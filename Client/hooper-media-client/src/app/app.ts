import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { LanguageService } from './core/services/language.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly languageService = inject(LanguageService);

  protected readonly title = signal('hooper-media-client');
  protected readonly isMobileMenuOpen = signal(false);
  protected readonly languages = this.languageService.languages;
  protected readonly currentLanguage = this.languageService.currentLanguage;

  protected toggleMobileMenu(): void {
    this.isMobileMenuOpen.update((isOpen) => !isOpen);
  }

  protected closeMobileMenu(): void {
    this.isMobileMenuOpen.set(false);
  }

  protected async updateLanguage(languageCode: string): Promise<void> {
    await this.languageService.setLanguage(languageCode);
  }

  protected translate(key: string): string {
    return this.languageService.translate(key);
  }
}
