import {Component, Input, numberAttribute, OnInit} from '@angular/core';
import {CreateExpense, CreateFullExpense, Group, GroupService, UserInGroup} from "../../group/group.service";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {FormBuilder, Validators} from "@angular/forms";
import {ToastController} from "@ionic/angular";


@Component({
  selector: 'app-createexpense',
  templateUrl: './createexpense.component.html',
  styleUrls: ['./createexpense.component.scss'],
})
export class CreateexpenseComponent  implements OnInit {
  userInGroup: UserInGroup[] = [];
  id: any;


  constructor(
    private route: ActivatedRoute,
    private readonly fb: FormBuilder,
    private readonly service: GroupService,
    private readonly toast: ToastController,
  ) {
  }

  async ngOnInit() {
    await this.getId()
    this.getUsersInGroup(this.id);
  }



  get description() {
    return this.form.controls.description;
  }

  get amount() {
    return this.form.controls.amount;
  }

  get createdDate() {
    return this.form.controls.createdDate;
  }

  async getId() {
    const map = await firstValueFrom(this.route.paramMap)
    this.id = map.get('groupId')
  }

  async getUsersInGroup(groupId: string) {
    this.userInGroup = await this.service.getUserInGroup(groupId)
  }

  usersOnExpense: number[] = [];

  handleUserSelection(event: CustomEvent) {
    if (event.detail.value !== undefined) {
      this.usersOnExpense = event.detail.value
    }
  }

  form = this.fb.group({

    description: ['', Validators.required],
    amount: [undefined, Validators.required],
    createdDate: [undefined, Validators.required]
  });

  async createExpense() {


    var expenseInfo: CreateExpense = {
      groupId: this.id,
      description: this.form.controls.description.value!,
      amount: this.form.controls.amount.value!,
      createdDate: this.form.controls.createdDate.value!

    };



    var fullExpenseInfo: CreateFullExpense = {
      expense: expenseInfo,
      userIdsOnExpense: this.usersOnExpense
    }

    const createdExpense = await firstValueFrom(this.service.createExpense(fullExpenseInfo));

    await (await this.toast.create({
      message: "Your expense '" + createdExpense.expense.description + "' was created successfully",
      color: "success",
      duration: 5000
    })).present();
  }






}
