import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';
import { AppStateService } from './shared/app-state/app-state.service';
import { Observable, Subscription } from 'rxjs';
import { IAppState } from './shared/app-state/app-state';
import { AsyncPipe } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatToolbarModule, MatButtonModule, MatMenuModule, MatIconModule, AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'CinemaxSPA';

  public isLoggedIn: boolean = false;

  public appState$: Observable<IAppState>;

  constructor(private appStateService: AppStateService) {
    this.appState$ = this.appStateService.getAppState();
  }

  ngOnInit(): void {}
}
