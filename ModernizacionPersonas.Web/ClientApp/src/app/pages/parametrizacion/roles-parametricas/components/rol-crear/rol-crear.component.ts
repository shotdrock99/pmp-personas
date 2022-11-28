import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Permiso, Rol } from "src/app/models/rol";
import { PermisosReaderService } from "src/app/services/permisos-reader.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RolEditarComponent } from "../rol-editar/rol-editar.component";

@Component({
  selector: "app-rol-crear",
  templateUrl: "./rol-crear.component.html",
  styleUrls: ["./rol-crear.component.scss"],
})

export class RolCrearComponent implements OnInit {

  createRolForm: FormGroup;
  title: string;
  permisos: Permiso[];
  permisosRol: Permiso[];

  constructor(
    private permisosReaderService: PermisosReaderService,
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<RolEditarComponent>,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;

  }

  ngOnInit() {
    this.loadPermisosRol();
    this.createRolForm = this.fb.group({
      nombre: [null, [Validators.required]],
      descripcion: [null, [Validators.required]],
      permisos: [null, []],
    });
  }

  loadPermisosRol() {
    this.permisosReaderService.getPermisos().subscribe((response) => {
      this.permisosRol = [
        {
          codigo: -1,
          descripcion: "",
          nombre: "Seleccione una opción...",
        },
        ...response,
      ];
      this.permisos = [this.permisosRol.find((x) => x.codigo == -1)];
    });
  }

  create() {
    if (!this.createRolForm.valid) {
      return;
    }
    this.dialogRef.close(this.createRolForm.value);
  }
}
