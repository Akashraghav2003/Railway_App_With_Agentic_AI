import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module'; 
import { AppComponent } from './app.component';
import { HomepageComponent } from './homepage/homepage.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { NavigationbarComponent } from './navigationbar/navigationbar.component';
import { TrainchartComponent } from './trainchart/trainchart.component';
import { SearchtrainComponent } from './searchtrain/searchtrain.component';
import { UserdashboardComponent } from './userdashboard/userdashboard.component';
import { AdmindashboardComponent } from './admindashboard/admindashboard.component';
import { AdminSidebarComponent } from './admin-sidebar/admin-sidebar.component';
import { AddtrainComponent } from './addtrain/addtrain.component';
import { UpdatetrainComponent } from './updatetrain/updatetrain.component';
import { UserSidebarComponent } from './userslidebar/userslidebar.component';
import { AddReservationComponent } from './addreservation/addreservation.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { UserdetailsComponent } from './userdetails/userdetails.component';
import { AdmindetailsComponent } from './admindetails/admindetails.component';
import { AllreservationsComponent } from './allreservations/allreservations.component';
import { CancellationTicketComponent } from './cancellation-ticket/cancellation-ticket.component';
import { CanceltrainComponent } from './canceltrain/canceltrain.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ChatbotComponent } from './chatbot/chatbot.component';



@NgModule({
  declarations: [
    AppComponent,
    HomepageComponent,
    LoginComponent,
    RegisterComponent,
    NavigationbarComponent,
    TrainchartComponent,
    SearchtrainComponent,
    UserdashboardComponent,
    AdmindashboardComponent,
    AdminSidebarComponent,
    AddtrainComponent,
    UpdatetrainComponent,
    UserSidebarComponent,
    AddReservationComponent,
    HeaderComponent,
    FooterComponent,
    UserdetailsComponent,
    AdmindetailsComponent,
    AllreservationsComponent,
    CancellationTicketComponent,
    HeaderComponent,
    FooterComponent,
    CanceltrainComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ChatbotComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
