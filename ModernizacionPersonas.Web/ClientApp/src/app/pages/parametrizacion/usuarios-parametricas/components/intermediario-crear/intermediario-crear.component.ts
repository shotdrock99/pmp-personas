import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { UsersReaderService } from "src/app/services/users-reader.service";
import { NotificationService } from "src/app/shared/services/notification.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import {UsuarioPersonas, RolUsuarioPersonas } from "src/app/models/usuario-personas";

@Component({
  selector: "app-intermediario-crear",
  templateUrl: "./intermediario-crear.component.html",
  styleUrls: ["./intermediario-crear.component.scss"],
})
export class IntermediarioCrearComponent implements OnInit {
  createForm: FormGroup;
  title: string;

  constructor(
    private usersReaderService: UsersReaderService,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private dialogRef: MatDialogRef<IntermediarioCrearComponent>,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;
  }

  ngOnInit() {
    this.createForm = this.fb.group({
      codigoIntermediario: [null, [Validators.required]],
      razonSocial: [null, []],
      email: [null, []],
    });
  }

  create() {
    if (!this.createForm.valid) {
      return;
    }
    let rolIntermediario: RolUsuarioPersonas = {
      roleId: 9,
      roleName: "Intermediario",
      rolDescription: "",
    };
    let response: UsuarioPersonas = {
      name: this.createForm.value.razonSocial,
      email: this.createForm.value.email,
      active: true,
      userName: this.createForm.value.codigoIntermediario,
      rol: rolIntermediario,
    };
    this.dialogRef.close(response);
  }

  change(event) {
    this.usersReaderService.validateIntermediario(event.target.value).subscribe(
      (response) => {
        this.createForm.patchValue({
          razonSocial: response.applicationUser.name,
          email: response.applicationUser.email,
        });
      },
      (error) => {
        if (error.status == 400) {
          this.notificationService.showToast(error.error.message);
          this.createForm.patchValue({
            codigoIntermediario: null
          })
        }
      }
    );
  }
}
