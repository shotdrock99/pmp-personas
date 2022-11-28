import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import {
  RolUsuarioPersonas,
  UsuarioPersonas,
} from "src/app/models/usuario-personas";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RolesUsuarioReaderService } from 'src/app/services/roles-usuario.reader.service';

@Component({
  selector: "app-usuario-editar",
  templateUrl: "./usuario-editar.component.html",
  styleUrls: ["./usuario-editar.component.scss"],
})
export class UsuarioEditarComponent implements OnInit {
  form: FormGroup;
  title: string;
  userId: number;
  name: string;
  email: string;
  userName: string;
  roles: number = 0;
  rolesUsuario: RolUsuarioPersonas[];

  constructor(
    private rolesUsuarioReaderService: RolesUsuarioReaderService,
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<UsuarioEditarComponent>,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;
    this.name = data.name;
    this.email = data.email;
    this.userId = data.userId;
    this.userName = data.userName;
    this.roles = data.rol.roleId;
  }

  ngOnInit() {
    this.loadRolesUsuario();
    this.form = this.fb.group({
      userId: [this.userId, []],
      name: [this.name, [Validators.required]],
      email: [this.email, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]],
      userName: [this.userName, []],
      roles: [this.roles, []],
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

  edit() {
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
}
