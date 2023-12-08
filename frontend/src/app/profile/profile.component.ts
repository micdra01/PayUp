import {Component, OnInit} from '@angular/core';
import {EditUserDto, EditUserImg, FullUser, ProfileService, TotalBalanceDto} from "./profile.service";
import {firstValueFrom, Observable} from "rxjs";
import {FormBuilder, Validators} from "@angular/forms";
import {ToastController} from "@ionic/angular";
import {HttpEventType} from "@angular/common/http";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent  implements OnInit {
  fullUser$!: Observable<FullUser>;
  editMode = false;
  editModeImg = false;
  editedUser!: EditUserDto;
  imageUrl: string | ArrayBuffer | null = null;
  uploading: boolean = false;
  totalBalance!: TotalBalanceDto;

  constructor(private readonly service: ProfileService,
              private readonly fb: FormBuilder,
              private readonly toast: ToastController,) {}

  async ngOnInit() {
    this.fullUser$ = this.service.getCurrentUser();
    const user = await firstValueFrom(this.fullUser$)
    this.totalBalance = await this.service.getTotalBalance();
    this.imageUrl = user.profileUrl;

  }

  form = this.fb.group({
    email: ['', Validators.required],
    fullName: ['', Validators.required],
    phone: ['', Validators.required],
  });

  formImg = this.fb.group({
    image: [null as File | null],
  })

  enterEditMode() {
    this.editMode = true;
    // Copy the values to the editedUser object
    this.fullUser$!.subscribe(user => {this.editedUser = { ...user}});
  }

  exitEditMode() {
    this.editMode = false;
  }

  async saveChanges() {
    var user: EditUserDto = {
      email: this.form.controls.email.value!,
      fullName: this.form.controls.fullName.value!,
      phoneNumber: this.form.controls.phone.value!,

    };

    this.fullUser$ = this.service.editCurrentUser(user as EditUserDto);

    await (await this.toast.create({
      message: "Your profile was updated successfully",
      color: "success",
      duration: 5000
    })).present();
    this.exitEditMode();
  }

  submit() {
    if (this.formImg.invalid) return;
    this.service.editUserImage(this.formImg.value as EditUserImg).pipe()
      .subscribe(event => {
        if (event.type == HttpEventType.Response && event.body) {
          this.formImg.get('image')?.setValue(event.body.imageUrl);
          location.reload()
        }
      });
  }

  onFileChanged($event: Event) {
    const files = ($event.target as HTMLInputElement).files;
    if (!files) return;
    this.uploading = true;
    this.formImg.patchValue({image: files[0]});
    this.formImg.controls.image.updateValueAndValidity();
    const reader = new FileReader();
    reader.readAsDataURL(files[0]);
    reader.onload = () => {
      this.imageUrl = reader.result;
    }
    this.uploading = false;
  }

  enterImgEditMode() {
    this.editModeImg = true;
  }

  exitImgEditMode() {
    this.editModeImg = false;
  }
}
