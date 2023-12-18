import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, ValidationErrors, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {AccountService, Credentials, Registration} from "../account.service";
import {TokenService} from "../../../services/TokenService";
import {ToastController} from "@ionic/angular";
import {Router} from "@angular/router";
import {ToolbarComponent} from "../../toolbar/toolbar.component";


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {

  readonly form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    repeatPassword: ['', [Validators.required]],
    phone: ['', Validators.required],
  }, {validators: this.isPasswordSame});

  constructor(
    private readonly fb: FormBuilder,
    private readonly token: TokenService,
    private readonly toast: ToastController,
    private readonly service: AccountService,
    private readonly router: Router,
    private readonly tb: ToolbarComponent
  ) {
  }

  get name() {
    return this.form.controls.name;
  }

  get email() {
    return this.form.controls.email;
  }

  get phone() {
    return this.form.controls.phone;
  }

  get password() {
    return this.form.controls.password;
  }

  get repeatPassword() {
    return this.form.controls.repeatPassword;
  }

  ngOnInit() {
  }

  async register() {

    let userInfo = this.createUserFromForm();
    if (this.form.invalid) return;
    const {any} = await firstValueFrom(this.service.register(userInfo as Registration));

    const {token} = await firstValueFrom(this.service.login(this.form.value as Credentials));
    this.token.setToken(token);

    await this.NavigateToHome();
  }
  createUserFromForm(){
    var userInfo: Registration =
      {
        created: new Date(Date.now()),
        email: this.form.controls.email.value!,
        fullName: this.form.controls.name.value!,
        password: this.form.controls.password.value!,
        phoneNumber: this.form.controls.phone.value!,
        profileUrl: "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava3.webp"
      }

      return userInfo;
  }

  async NavigateToHome() {
    await (await this.toast.create({
      message: "Welcome to PayUp!",
      color: "success",
      duration: 5000
    })).present();
    await this.router.navigate(['/groups'])
    //Refresh toolbar to show logged-in user
    this.tb.loggedInUser = await this.service.getCurrentUser()
    location.reload()
  }

  isPasswordSame(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const repeatPassword = control.get('repeatPassword')?.value;

    return password === repeatPassword ? null : {passwordsNotMatch: false};
  }
}


