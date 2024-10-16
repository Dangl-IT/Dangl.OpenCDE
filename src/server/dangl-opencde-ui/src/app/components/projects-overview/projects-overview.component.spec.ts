import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsOverviewComponent } from './projects-overview.component';
import { SharedTestingModule } from 'src/app/shared-tesing.module';

describe('ProjectsOverviewComponent', () => {
  let component: ProjectsOverviewComponent;
  let fixture: ComponentFixture<ProjectsOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProjectsOverviewComponent],
      imports: [SharedTestingModule],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
