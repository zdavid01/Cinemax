import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { map, take } from 'rxjs/operators';
import { AppStateService } from '../app-state/app-state.service';

export const premiumGuard: CanActivateFn = (route, state) => {
  const appStateService = inject(AppStateService);
  const router = inject(Router);

  return appStateService.getAppState().pipe(
    take(1),
    map(appState => {
      if (appState.isPremium) {
        return true;
      } else {
        // Redirect to premium page if not a premium user
        router.navigate(['/premium']);
        return false;
      }
    })
  );
};

