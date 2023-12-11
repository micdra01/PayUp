import {NgModule} from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import {CreateComponent} from "./create/create.component";
import {IonicModule} from "@ionic/angular";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {GroupService} from "./group.service";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {ErrorHttpInterceptor} from "../../interceptors/error-http-interceptors";
import {InviteComponent} from "./invite/invite.component";

import {ExpenseModule} from "../expense/expense.module";
import {ActivityComponent} from "./activity/activity.component";
import {GroupcardComponent} from "./groupcard/groupcard.component";
import {BalancecardComponent} from "./balancecard/balancecard.component";
import {MyGroupsComponent} from "./my-groups/my-groups.component";
import {UpdateComponent} from "./update/update.component";
import {PayerTransforCardComponent} from "./balancecard/payer-transfor-card/payer-transfor-card.component";
import {PayeeTransforCardComponent} from "./balancecard/payee-transfor-card/payee-transfor-card.component";
import {MembercardComponent} from "./membercard/membercard.component";




@NgModule({
    declarations: [
        CreateComponent,
        ActivityComponent,
        GroupcardComponent,
        InviteComponent,
        BalancecardComponent,
        MyGroupsComponent,
        UpdateComponent,
        PayerTransforCardComponent,
        PayeeTransforCardComponent,
        MembercardComponent
    ],
    imports: [
        CommonModule,
        IonicModule,
        ReactiveFormsModule,
        ExpenseModule,
        FormsModule,
        NgOptimizedImage
    ],
  exports: [],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorHttpInterceptor, multi: true},
    GroupService
  ]
})
export class GroupModule {
}
