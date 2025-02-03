import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivateSessionComponent } from './private-session.component';

describe('PrivateSessionComponent', () => {
  let component: PrivateSessionComponent;
  let fixture: ComponentFixture<PrivateSessionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrivateSessionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrivateSessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
