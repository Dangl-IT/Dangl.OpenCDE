import { RouterModule, Routes } from '@angular/router';

import { MainComponent } from './components/main/main.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: '',
    component: MainComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
