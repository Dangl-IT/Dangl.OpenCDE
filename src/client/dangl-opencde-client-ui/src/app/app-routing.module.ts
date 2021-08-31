import { RouterModule, Routes } from '@angular/router';

import { MainStepperComponent } from './components/main-stepper/main-stepper.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: '',
    component: MainStepperComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
