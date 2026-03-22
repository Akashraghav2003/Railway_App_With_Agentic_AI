import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { NavigationbarComponent } from './navigationbar/navigationbar.component';
import { TrainchartComponent } from './trainchart/trainchart.component';
import { SearchtrainComponent } from './searchtrain/searchtrain.component';
import { UserdashboardComponent } from './userdashboard/userdashboard.component';
import { AdmindashboardComponent } from './admindashboard/admindashboard.component';
import { AddtrainComponent } from './addtrain/addtrain.component';
import { UpdatetrainComponent } from './updatetrain/updatetrain.component';
import { AddReservationComponent } from './addreservation/addreservation.component';
import { UserdetailsComponent } from './userdetails/userdetails.component';
import { AllreservationsComponent } from './allreservations/allreservations.component';
import { CancellationTicketComponent } from './cancellation-ticket/cancellation-ticket.component';
import { AdmindetailsComponent } from './admindetails/admindetails.component';
import { CanceltrainComponent } from './canceltrain/canceltrain.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { AuthGuard } from './guards/auth.guard';


const routes: Routes = [
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: HomepageComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'navigation', component: NavigationbarComponent },
  { path: 'trainchart', component: TrainchartComponent },
  { path: 'searchtrain', component: SearchtrainComponent },
  { path: 'userdashboard', component: UserdashboardComponent, canActivate: [AuthGuard] },
  { path: 'admindashboard', component: AdmindashboardComponent, canActivate: [AuthGuard] },
  {path:'addtrain', component:AddtrainComponent, canActivate: [AuthGuard]},
  {path:'updatetrain', component:UpdatetrainComponent, canActivate: [AuthGuard]},
  {path:'addreservation', component:AddReservationComponent, canActivate: [AuthGuard]},
  {path:'userdetails', component:UserdetailsComponent, canActivate: [AuthGuard]},
  {path:'allreservations', component:AllreservationsComponent, canActivate: [AuthGuard]},
  {path:'cancelreservation', component: CancellationTicketComponent, canActivate: [AuthGuard]},
  {path:'Admindetails', component: AdmindetailsComponent, canActivate: [AuthGuard]},
  {path:'canceltrain', component: CanceltrainComponent, canActivate: [AuthGuard]},
  {path:'forgot-password', component: ForgotPasswordComponent},
  {path:'reset-password', component: ResetPasswordComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
