import {NgModule} from '@angular/core';
import {PreloadAllModules, RouterModule, Routes} from '@angular/router';
import {RegisterComponent} from "./auth/register/register.component";
import {LoginComponent} from "./auth/login/login.component";
import {InviteComponent} from "./group/invite/invite.component";
import {CreateComponent} from "./group/create/create.component";
import {ProfileComponent} from "./profile/profile.component";
import {ActivityComponent} from "./group/activity/activity.component";
import {MyGroupsComponent} from "./group/my-groups/my-groups.component";
import {HomePage} from "./home/home.page";
import {NotificationComponent} from "./notification/notification.component";
import {CreateexpenseComponent} from "./expense/createexpense/createexpense.component";
import {AuthGuard} from "../services/AuthGuard";
import {UpdateComponent} from "./group/update/update.component";
import {SettleComponent} from "./expense/settle/settle.component";


const routes: Routes = [
  {
    path: '',
    redirectTo: 'groups',
    pathMatch: 'full'
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'groups',
    component: MyGroupsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/:groupid/invite',
    component: InviteComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/:groupId/edit',
    component: UpdateComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/create',
    component: CreateComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'user/notifications',
    component: NotificationComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/:groupId',
    component: ActivityComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/:groupId/create',
    component: CreateexpenseComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups/:groupId/settle',
    component: SettleComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {preloadingStrategy: PreloadAllModules})
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
