using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.HID;
using WebVisitsMobile.Domain.Entities.Organizacion.Tarea;
using WebVisitsMobile.Domain.EntitiesCustom;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Models.HID.PlantillaCredencial;
using WebVisitsMobile.Models.Organizacion.Tarea.Tarea;
using WebVisitsMobile.Services.Interfaces.HID;
using WebVisitsMobile.Services.QueryFilters.HID;

namespace WebVisitsMobile.Services.Services.HID
{
    public class PlantillaCredencialService : IPlantillaCredencialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOption _paginationOptions;
        private readonly IHostEnvironment _env;
        public PlantillaCredencialService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options,
            IHostEnvironment env
            )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _env = env;
        }

        //public async Task<PlantillaCredencial?> GetById(Guid id, Guid empresaId)
        //{
        //    try
        //    {
        //        PlantillaCredencial perfil = await _unitOfWork.PlantillaCredencialRepository.GetById(id);
        //        if (empresaId == Guid.Empty || perfil == null)
        //        {
        //            return null;
        //        }

        //        return perfil;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<PlantillaCredencial?> GetById(Guid id, Guid empresaId)
        {
            try
            {
                PlantillaCredencial perfil = await _unitOfWork.PlantillaCredencialRepository.GetById(id);
                if (empresaId == Guid.Empty || perfil == null)
                {
                    return null;
                }

                // Cargar las imágenes en Base64 para preview
                string baseFolder = Path.Combine(_env.ContentRootPath, "FOTOS_PLANTILLAS_CREDENCIALES");

                // Imagen de fondo
                if (!string.IsNullOrEmpty(perfil.ImagenFondo) && perfil.ImagenFondo != "Sin foto")
                {
                    string fondoPath = Path.Combine(baseFolder, perfil.ImagenFondo);
                    perfil.ImagenFondoBase64 = await GetImageBase64(fondoPath);
                }

                // Imagen de logo
                if (!string.IsNullOrEmpty(perfil.ImagenLogo) && perfil.ImagenLogo != "Sin foto")
                {
                    string logoPath = Path.Combine(baseFolder, perfil.ImagenLogo);
                    perfil.ImagenLogoBase64 = await GetImageBase64(logoPath);
                }

                return perfil;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Método auxiliar para convertir un archivo de imagen a Base64
        private async Task<string?> GetImageBase64(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string extension = Path.GetExtension(filePath).TrimStart('.').ToLower();
            string mimeType = extension switch
            {
                "png" => "image/png",
                "jpg" or "jpeg" => "image/jpeg",
                "webp" => "image/webp",
                "svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
            return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
        }

        public PagedList<PlantillaCredencial> GetAll(PlantillaCredencialQueryFilter filters, Guid empresaId)
        {
            try
            {
                filters.PageNumber = filters.PageNumber == 0 ? int.Parse(_paginationOptions.DefaultPageNumber) : filters.PageNumber;
                filters.PageSize = filters.PageSize == 0 ? int.Parse(_paginationOptions.DefaultPageSize) : filters.PageSize;

                IEnumerable<PlantillaCredencial> data;

                if (filters.DatosCompletos == 0)
                {
                    data = _unitOfWork.PlantillaCredencialRepository.GetAll();
                }
                else
                {
                    data = _unitOfWork.PlantillaCredencialRepository.GetAll();
                }


                if (!string.IsNullOrWhiteSpace(filters.Nombre))
                {
                    data = data
                        .Where(x => !string.IsNullOrWhiteSpace(x.Nombre) &&
                                    x.Nombre.Contains(filters.Nombre, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(filters.ImagenFondo))
                {
                    data = data
                        .Where(x => !string.IsNullOrWhiteSpace(x.ImagenFondo) &&
                                    x.ImagenFondo.Contains(filters.ImagenFondo, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(filters.ExtensionImagenFondo))
                {
                    data = data
                        .Where(x => !string.IsNullOrWhiteSpace(x.ExtensionImagenFondo) &&
                                    x.ExtensionImagenFondo.Contains(filters.ExtensionImagenFondo, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(filters.ImagenLogo))
                {
                    data = data
                        .Where(x => !string.IsNullOrWhiteSpace(x.ImagenLogo) &&
                                    x.ImagenLogo.Contains(filters.ImagenLogo, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrWhiteSpace(filters.ExtensionImagenLogo))
                {
                    data = data
                        .Where(x => !string.IsNullOrWhiteSpace(x.ExtensionImagenLogo) &&
                                    x.ExtensionImagenLogo.Contains(filters.ExtensionImagenLogo, StringComparison.OrdinalIgnoreCase));
                }

                if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { data = data.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
                if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { data = data.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
                if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { data = data.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
                if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { data = data.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
                if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
                if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
                if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
                if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
                if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
                if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
                if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
                if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { data = data.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
                if (filters.Estado != null && filters.Estado > 0) { data = data.Where(x => x.Estado == filters.Estado); }

                var paged = PagedList<PlantillaCredencial>.Create(data, filters.PageNumber, filters.PageSize);

                return paged;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Inactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                PlantillaCredencial data = await _unitOfWork.PlantillaCredencialRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaBaja = DateTime.Now;
                data.UsuarioBajaId = currentUserId;
                data.Estado = 2;

                _unitOfWork.PlantillaCredencialRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Reactivate(Guid id, Guid currentUserId)
        {
            bool booOk = false;
            try
            {
                PlantillaCredencial data = await _unitOfWork.PlantillaCredencialRepository.GetById(id);
                if (data == null) { return false; }
                data.FechaReactivacion = DateTime.Now;
                data.UsuarioReactivadorId = currentUserId;
                data.Estado = 1;

                _unitOfWork.PlantillaCredencialRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Update(Guid id, PlantillaCredencialExternalReqDTO credencial)
        {
            bool booOk = false;
            try
            {
                PlantillaCredencial data = await _unitOfWork.PlantillaCredencialRepository.GetById(id);
                if (data == null) { return false; }

                data.BackgroundExternalId = credencial.BackgroundExternalId;
                data.LogoExternalId = credencial.LogoExternalId;
                data.ExternalId = credencial.ExternalId;

                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificadorId = credencial.UsuarioModificadorId;

                _unitOfWork.PlantillaCredencialRepository.Update(data);
                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> Create(PlantillaCredencial data, Guid currentUserId, Guid clientCompanyId)
        {
            bool booOk = false;

            try
            {
                if (currentUserId == Guid.Empty) { return false; }
                // 1. Obtener la ruta base dinámica usando IHostEnvironment
                string baseFolder = Path.Combine(_env.ContentRootPath, "FOTOS_PLANTILLAS_CREDENCIALES");
                // 2. Crear la carpeta si no existe
                Directory.CreateDirectory(baseFolder);

                var imagenFondo = data.ImagenFondo;
                var idImagenFondo = Guid.NewGuid();

                if (imagenFondo == null || imagenFondo == "")
                {
                    data.ImagenFondo = "Sin foto";
                }
                else
                {
                    //Se crea el nombre de la foto que se compone por el ID de colaborador y la extencion de la foto
                    string imageName = idImagenFondo.ToString() + '.' + data.ExtensionImagenFondo;

                    //Se forma el path donde estara la imagen almacenada en nuestro servidor
                    string imgPath = Path.Combine("FOTOS_PLANTILLAS_CREDENCIALES", imageName);

                    //La imagen base 64 se trasforma a una imagen por bytes
                    byte[] imageBytes = Convert.FromBase64String(imagenFondo);

                    //La foto se escribe dentro de la carpeta seleccionada por  medio del path
                    File.WriteAllBytes(imgPath, imageBytes);

                    //En colaborador foto se guarda el nombre de la foto
                    data.ImagenFondo = idImagenFondo.ToString() + '.' + data.ExtensionImagenFondo;
                }

                var imagenLogo = data.ImagenLogo;
                var idImagenLogo = Guid.NewGuid();

                if (imagenLogo == null || imagenLogo == "")
                {
                    data.ImagenLogo = "Sin foto";
                }
                else
                {
                    //Se crea el nombre de la foto que se compone por el ID de colaborador y la extencion de la foto
                    string imageName = idImagenLogo.ToString() + '.' + data.ExtensionImagenLogo;

                    //Se forma el path donde estara la imagen almacenada en nuestro servidor
                    string imgPath = Path.Combine("FOTOS_PLANTILLAS_CREDENCIALES", imageName);

                    //La imagen base 64 se trasforma a una imagen por bytes
                    byte[] imageBytes = Convert.FromBase64String(imagenLogo);

                    //La foto se escribe dentro de la carpeta seleccionada por  medio del path
                    File.WriteAllBytes(imgPath, imageBytes);

                    //En colaborador foto se guarda el nombre de la foto
                    data.ImagenLogo = idImagenLogo.ToString() + '.' + data.ExtensionImagenLogo;
                }

                data.Id = Guid.NewGuid();

                if (data.BackgroundExternalId == null)
                {
                    data.BackgroundExternalId = data.Id;
                }
                if (data.LogoExternalId == null)
                {
                    data.LogoExternalId = data.Id;
                }
                if (data.ExternalId == null)
                {
                    data.ExternalId = data.Id;
                }
                if (data.AppleId == null)
                {
                    data.AppleId = data.Id;
                }

                data.Estado = 1;
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreadorId = currentUserId;

                await _unitOfWork.PlantillaCredencialRepository.Add(data);

                var typeTask = await _unitOfWork.TipoTareaRepository.GetById(new Guid("022047D9-2E61-44A5-ADE4-AEB2F69CDC17"));
                if (typeTask != null)
                {
                    var template = new TareaPlantillaDTO()
                    {
                        Id = data.Id,
                        NombrePlantilla = data.Nombre,
                        //ImagenFondo = imagenFondo ?? "",
                        //ExtensionImagenFondo = data.ExtensionImagenFondo ?? "",
                        //ImagenLogo = imagenLogo ?? "",
                        //ExtensionImagenLogo = data.ExtensionImagenLogo ?? ""
                    };

                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    Tarea task = new Tarea
                    {
                        TipoTareaId = typeTask.Id,
                        Fecha = DateTime.Now,
                        Pendiente = 1,
                        Status = 1,
                        ValorEnvio = JsonSerializer.Serialize(template, jsonOptions),
                        ValorRetorno = "",
                        ReferenciaId = data.Id,
                        EmpresaClienteId = clientCompanyId,
                        Marca = 0,
                        Id = Guid.NewGuid(),
                        UsuarioCreadorId = currentUserId,
                        FechaCreacion = DateTime.Now,
                        Estado = 1
                    };

                    await _unitOfWork.TareaRepository.Add(task);
                }

                await _unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        //public async Task<bool> Update(PlantillaCredencial data, Guid currentUserId, Guid clientCompanyId)
        //{
        //    try
        //    {
        //        if (currentUserId == Guid.Empty) { return false; }
        //        if (data.Id == Guid.Empty) { return false; }

        //        PlantillaCredencial currentData = await _unitOfWork.PlantillaCredencialRepository.GetById(data.Id);

        //        if (currentData == null) { return false; }

        //        currentData.Nombre = data.Nombre;
        //        currentData.ImagenFondo = data.ImagenFondo;
        //        currentData.ExtensionImagenFondo = data.ExtensionImagenFondo;
        //        currentData.ImagenLogo = data.ImagenLogo;
        //        currentData.ExtensionImagenLogo = data.ExtensionImagenLogo;


        //        currentData.FechaModificacion = DateTime.Now;
        //        currentData.UsuarioModificadorId = currentUserId;

        //        _unitOfWork.PlantillaCredencialRepository.Update(currentData);

        //        await _unitOfWork.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> Update(PlantillaCredencial data, Guid currentUserId, Guid clientCompanyId)
        {
            if (currentUserId == Guid.Empty || data.Id == Guid.Empty)
                return false;

            var currentData = await _unitOfWork.PlantillaCredencialRepository.GetById(data.Id);
            if (currentData == null)
                return false;

            // Carpeta base para las imágenes
            string baseFolder = Path.Combine(_env.ContentRootPath, "FOTOS_PLANTILLAS_CREDENCIALES");
            Directory.CreateDirectory(baseFolder); // por si acaso no existe

            // ---------- IMAGEN FONDO ----------
            string nuevaImagenFondo = data.ImagenFondo; // lo que venga del DTO
            string fondoActual = currentData.ImagenFondo;
            string extensionFondo = data.ExtensionImagenFondo ?? currentData.ExtensionImagenFondo;

            if (string.IsNullOrWhiteSpace(nuevaImagenFondo) || nuevaImagenFondo == "Sin foto")
            {
                // Se eliminó la imagen de fondo
                EliminarImagenSiExiste(baseFolder, fondoActual);
                currentData.ImagenFondo = "Sin foto";
                currentData.ExtensionImagenFondo = null;
            }
            else if (EsBase64(nuevaImagenFondo))
            {
                // Es una nueva imagen en Base64
                EliminarImagenSiExiste(baseFolder, fondoActual);
                string nuevoNombre = $"{Guid.NewGuid()}.{extensionFondo}";
                GuardarImagenBase64(baseFolder, nuevoNombre, nuevaImagenFondo);
                currentData.ImagenFondo = nuevoNombre;
                currentData.ExtensionImagenFondo = extensionFondo;
            }
            else
            {
                // No se modificó (viene el nombre de archivo original)
                currentData.ImagenFondo = nuevaImagenFondo;
                currentData.ExtensionImagenFondo = extensionFondo;
            }

            // ---------- IMAGEN LOGO (misma lógica) ----------
            string nuevaImagenLogo = data.ImagenLogo;
            string logoActual = currentData.ImagenLogo;
            string extensionLogo = data.ExtensionImagenLogo ?? currentData.ExtensionImagenLogo;

            if (string.IsNullOrWhiteSpace(nuevaImagenLogo) || nuevaImagenLogo == "Sin foto")
            {
                EliminarImagenSiExiste(baseFolder, logoActual);
                currentData.ImagenLogo = "Sin foto";
                currentData.ExtensionImagenLogo = null;
            }
            else if (EsBase64(nuevaImagenLogo))
            {
                EliminarImagenSiExiste(baseFolder, logoActual);
                string nuevoNombre = $"{Guid.NewGuid()}.{extensionLogo}";
                GuardarImagenBase64(baseFolder, nuevoNombre, nuevaImagenLogo);
                currentData.ImagenLogo = nuevoNombre;
                currentData.ExtensionImagenLogo = extensionLogo;
            }
            else
            {
                currentData.ImagenLogo = nuevaImagenLogo;
                currentData.ExtensionImagenLogo = extensionLogo;
            }

            // ---------- ACTUALIZAR OTROS CAMPOS ----------
            currentData.Nombre = data.Nombre;
            currentData.FechaModificacion = DateTime.Now;
            currentData.UsuarioModificadorId = currentUserId;

            _unitOfWork.PlantillaCredencialRepository.Update(currentData);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private bool EsBase64(string cadena)
        {
            if (string.IsNullOrEmpty(cadena)) return false;
            // Si empieza con "data:" o tiene solo caracteres Base64
            return cadena.StartsWith("data:") || cadena.Length > 100 && cadena.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=');
        }

        private void EliminarImagenSiExiste(string carpeta, string? nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo) || nombreArchivo == "Sin foto") return;
            string ruta = Path.Combine(carpeta, nombreArchivo);
            if (File.Exists(ruta)) File.Delete(ruta);
        }

        private void GuardarImagenBase64(string carpeta, string nombreArchivo, string base64)
        {
            // Si viene con prefijo "data:image/png;base64," lo limpiamos
            if (base64.Contains(','))
                base64 = base64.Substring(base64.IndexOf(',') + 1);
            byte[] bytes = Convert.FromBase64String(base64);
            string ruta = Path.Combine(carpeta, nombreArchivo);
            File.WriteAllBytes(ruta, bytes);
        }
    }
}