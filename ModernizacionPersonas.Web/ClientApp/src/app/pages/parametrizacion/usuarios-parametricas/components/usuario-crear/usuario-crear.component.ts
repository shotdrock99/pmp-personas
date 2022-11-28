import { Component, OnInit, Inject } from "@angular/core";
import { RolesUsuarioReaderService } from "src/app/services/roles-usuario.reader.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import {
  RolUsuarioPersonas,
  UsuarioPersonas,
} from "src/app/models/usuario-personas";
import { UsersReaderService } from 'src/app/services/users-reader.service';
import { NotificationService } from 'src/app/shared/services/notification.service';

@Component({
  selector: "app-usuario-crear",
  templateUrl: "./usuario-crear.component.html",
  styleUrls: ["./usuario-crear.component.scss"],
})
export class UsuarioCrearComponent implements OnInit {
  form: FormGroup;
  title: string;
  roles: number = 0;
  rolesUsuario: RolUsuarioPersonas[];

  constructor(
    private rolesUsuarioReaderService: RolesUsuarioReaderService,
    private usersReaderService: UsersReaderService,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private dialogRef: MatDialogRef<UsuarioCrearComponent>,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;
  }

  ngOnInit() {
    this.loadRolesUsuario();
    this.form = this.fb.group({
      userName: [null, [Validators.required]],
      name: [null, []],
      email: [null, []],
      roles: [null, [Validators.required]],
    });
  }

  loadRolesUsuario() {
    this.rolesUsuarioReaderService.getRolesUsuario().subscribe((response) => {
      this.rolesUsuario = [
        { roleId: 0, rolDescription: "", roleName: "Seleccione una opción..." },
        ...response.map((rol) => {
          return {
            roleId: rol.codigo,
            rolDescription: rol.descripcion,
            roleName: rol.nombre,
          };
        }),
      ];
    });
  }

  create() {
    if (!this.form.valid) {
      return;
    }
    let response: UsuarioPersonas = {
      userId: this.form.value.userId,
      name: this.form.value.name,
      email: this.form.value.email,
      active: this.form.value.activo,
      userName: this.form.value.userName,
      rol: this.rolesUsuario.find((x) => x.roleId == this.roles),
    };
    this.dialogRef.close(response);
  }

  change(event) {
    this.usersReaderService.validateUser(event.target.value)
      .subscribe(response => {
        this.form.patchValue({
          name: response.applicationUser.externalInfo.nombreUsuario,
          email: response.applicationUser.externalInfo.emailUsuario
        })
      },
        (error) => {
          if (error.status == 400)
            this.notificationService.showToast(error.error.message, 2000);
          this.form.patchValue({
            userName: null
          })
        });
  }
}
