using Microsoft.Extensions.Options;
using WebVisitsMobile.Data.Interfaces.Common;
using WebVisitsMobile.Domain.Entities.Parametrizacion;
using WebVisitsMobile.Domain.Options;
using WebVisitsMobile.Services.Interfaces.Parametrizacion;

namespace WebVisitsMobile.Services.Services.Parametrizacion
{
    public class CorreoEnviarService : ICorreoEnviarService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PaginationOption paginationOption;
        public CorreoEnviarService(
            IUnitOfWork unitOfWork,
            IOptions<PaginationOption> options
            )
        {
            this.unitOfWork = unitOfWork;
            paginationOption = options.Value;
        }

        //public async Task<PagedList<CorreoEnviar>> GetAllCorreoEnviar(CorreoEnviarQueryFilter filters, Guid EmpresaId)
        //{
        //    PagedList<CorreoEnviar> pagedSecciones = null;

        //    try
        //    {
        //        List<Guid> listaPermitidas = new List<Guid>();


        //        filters.PageNumber = filters.PageNumber == 0 ? int.Parse(paginationOptions.DefaultPageNumber) : filters.PageNumber;
        //        filters.PageSize = filters.PageSize == 0 ? int.Parse(paginationOptions.DefaultPageSize) : filters.PageSize;

        //        var correoEnviar = unitOfWork.CorreoEnviarRepository.GetAll();

        //        if (filters.De != null) { correoEnviar = correoEnviar.Where(x => x.De.ToLower().Contains(filters.De.ToLower())); }
        //        if (filters.Para != null) { correoEnviar = correoEnviar.Where(x => x.Para.ToLower().Contains(filters.Para.ToLower())); }
        //        if (filters.Cc != null) { correoEnviar = correoEnviar.Where(x => x.Cc.ToLower().Contains(filters.Cc.ToLower())); }
        //        if (filters.Mensaje != null) { correoEnviar = correoEnviar.Where(x => x.Mensaje.ToLower().Contains(filters.Mensaje.ToLower())); }
        //        if (filters.Enviado != null && filters.Enviado > 0) { correoEnviar = correoEnviar.Where(x => x.Enviado == filters.Enviado); }
        //        if (filters.Marca != null && filters.Marca > 0) { correoEnviar = correoEnviar.Where(x => x.Marca == filters.Marca); }
        //        if (filters.Asunto != null) { correoEnviar = correoEnviar.Where(x => x.Asunto.ToLower().Contains(filters.Asunto.ToLower())); }


        //        if (filters.UsuarioCreadorId != null && filters.UsuarioCreadorId != Guid.Empty) { correoEnviar = correoEnviar.Where(x => x.UsuarioCreadorId == filters.UsuarioCreadorId); }
        //        if (filters.UsuarioModificadorId != null && filters.UsuarioModificadorId != Guid.Empty) { correoEnviar = correoEnviar.Where(x => x.UsuarioModificadorId == filters.UsuarioModificadorId); }
        //        if (filters.UsuarioBajaId != null && filters.UsuarioBajaId != Guid.Empty) { correoEnviar = correoEnviar.Where(x => x.UsuarioBajaId == filters.UsuarioBajaId); }
        //        if (filters.UsuarioReactivadorId != null && filters.UsuarioReactivadorId != Guid.Empty) { correoEnviar = correoEnviar.Where(x => x.UsuarioReactivadorId == filters.UsuarioReactivadorId); }
        //        if (filters.FechaCreacionDesde != null && filters.FechaCreacionDesde != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionDesde) >= 0); }
        //        if (filters.FechaCreacionHasta != null && filters.FechaCreacionHasta != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaCreacionHasta) <= 0); }
        //        if (filters.FechaModificacionDesde != null && filters.FechaModificacionDesde != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionDesde) >= 0); }
        //        if (filters.FechaModificacionHasta != null && filters.FechaModificacionHasta != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaModificacionHasta) <= 0); }
        //        if (filters.FechaBajaDesde != null && filters.FechaBajaDesde != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaDesde) >= 0); }
        //        if (filters.FechaBajaHasta != null && filters.FechaBajaHasta != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaBajaHasta) <= 0); }
        //        if (filters.FechaReactivacionDesde != null && filters.FechaReactivacionDesde != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionDesde) >= 0); }
        //        if (filters.FechaReactivacionHasta != null && filters.FechaReactivacionHasta != DateTime.MinValue) { correoEnviar = correoEnviar.Where(x => x.FechaCreacion.CompareTo(filters.FechaReactivacionHasta) <= 0); }
        //        if (filters.Estado != null && filters.Estado > 0) { correoEnviar = correoEnviar.Where(x => x.Estado == filters.Estado); }
        //        if (filters.EmpresaId != null && filters.EmpresaId != Guid.Empty) { correoEnviar = correoEnviar.Where(x => x.EmpresaId == filters.EmpresaId); }

        //        // Ubicaciones = Ubicaciones.Where(x => listaPermitidas.Contains(x.Id));
        //        //correoEnviar = correoEnviar.Where(x => x.EmpresaId == EmpresaId);

        //        pagedSecciones = PagedList<CorreoEnviar>.Create(correoEnviar, filters.PageNumber, filters.PageSize);
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "GetAllCorreoEnviar";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, new Guid("03340F13-CED3-4893-83F1-25A783C5A772"), EmpresaId);
        //    }

        //    return pagedSecciones;
        //}

        //public async Task<CorreoEnviar> GetCorreoEnviar(Guid id)
        //{
        //    CorreoEnviar correoEnviar = new CorreoEnviar();

        //    try
        //    {
        //        correoEnviar = await unitOfWork.CorreoEnviarRepository.GetById(id);

        //    }
        //    catch (Exception ex)
        //    {

        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "GetAllCorreoEnviar";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, new Guid("03340F13-CED3-4893-83F1-25A783C5A772"), new Guid("2DABCD06-394F-4CD6-A0D6-B5B4150C073E"));
        //    }
        //    return correoEnviar;
        //}

        public async Task<List<CorreoEnviarConfiguracion>> GetPendingEmailsWithConfig()
        {
            var emails = await unitOfWork.CorreoEnviarRepository.GetPendingEmailsWithConfig();
            return emails ?? new List<CorreoEnviarConfiguracion>();
        }

        public async Task<bool> InsertEmailSend(CorreoEnviar correoEnviar, Guid UsuarioActualId, Guid empresaId)
        {
            bool booOk = false;

            try
            {

                correoEnviar.Id = Guid.NewGuid();
                correoEnviar.Estado = 1;
                correoEnviar.FechaCreacion = DateTime.Now;
                correoEnviar.UsuarioCreadorId = UsuarioActualId;

                correoEnviar.EmpresaClienteId = new Guid("E096DCEF-B118-4596-9FA0-676855A3FB53");

                await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);
                await unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public async Task<bool> MarkAsSent(Guid id)
        {
            bool booOk = false;

            try
            {

                CorreoEnviar correoEnviar = await GetEmailSend(id);

                if (correoEnviar == null) { return false; }

                correoEnviar.Enviado = 1;

                unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

                await unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<bool> IncreaseAttemptCount(Guid id)
        {
            bool booOk = false;

            try
            {
                CorreoEnviar correoEnviar = await GetEmailSend(id);

                if (correoEnviar == null) { return false; }

                correoEnviar.Marca = (byte)((correoEnviar.Marca ?? 0) + 1);

                unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

                await unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return booOk;
        }

        public async Task<CorreoEnviar> GetEmailSend(Guid id)
        {
            CorreoEnviar correoEnviar = new CorreoEnviar();

            try
            {
                correoEnviar = await unitOfWork.CorreoEnviarRepository.GetById(id);
            }
            catch (Exception ex)
            {
            }
            return correoEnviar;
        }

        //public async Task<bool> UpdateCorreoEnviar(CorreoEnviar correoEnviar, Guid usuarioActualId, Guid empresaId)
        //{
        //    try
        //    {
        //        if (empresaId == Guid.Empty || usuarioActualId == Guid.Empty) { return false; }

        //        CorreoEnviar correoEnviarCurrent = await unitOfWork.CorreoEnviarRepository.GetById(correoEnviar.Id);

        //        if (correoEnviarCurrent.EmpresaId != empresaId) { return false; }

        //        correoEnviarCurrent.De = correoEnviar.De;
        //        correoEnviarCurrent.Para = correoEnviar.Para;
        //        correoEnviarCurrent.Cc = correoEnviar.Cc;
        //        correoEnviarCurrent.Mensaje = correoEnviar.Mensaje;
        //        correoEnviarCurrent.Enviado = correoEnviar.Enviado;
        //        correoEnviarCurrent.Marca = correoEnviar.Marca;
        //        correoEnviarCurrent.Asunto = correoEnviar.Asunto;


        //        correoEnviarCurrent.FechaModificacion = DateTime.Now;
        //        correoEnviarCurrent.UsuarioModificadorId = usuarioActualId;

        //        unitOfWork.CorreoEnviarRepository.Update(correoEnviarCurrent);

        //        await unitOfWork.SaveChangesAsync();



        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "UpdateCorreoEnviar";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, usuarioActualId, empresaId);
        //        return false;
        //    }
        //}

        //public async Task<bool> ReactivarCorreoEnviar(Guid id, Guid usuarioActualId, Guid empresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {
        //        if (usuarioActualId == Guid.Empty || empresaId == Guid.Empty) { return false; }

        //        CorreoEnviar correoEnviar = await GetCorreoEnviar(id);

        //        if (correoEnviar == null) { return false; }

        //        if (correoEnviar.EmpresaId != empresaId) { return false; }

        //        correoEnviar.FechaBaja = DateTime.Now;
        //        correoEnviar.UsuarioBajaId = usuarioActualId;
        //        correoEnviar.Estado = 1;

        //        unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "ReactivarCorreoEnviar";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, usuarioActualId, empresaId);
        //        throw;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InactivarCorreoEnviar(Guid id, Guid usuarioActualId, Guid empresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {
        //        if (usuarioActualId == Guid.Empty || empresaId == Guid.Empty) { return false; }

        //        CorreoEnviar correoEnviar = await GetCorreoEnviar(id);

        //        if (correoEnviar == null) { return false; }

        //        if (correoEnviar.EmpresaId != empresaId) { return false; }

        //        correoEnviar.FechaBaja = DateTime.Now;
        //        correoEnviar.UsuarioBajaId = usuarioActualId;
        //        correoEnviar.Estado = 2;

        //        unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return booOk;
        //}

        //public IEnumerable<CorreoEnviar> GetCorreosPendientes()
        //{
        //    IEnumerable<CorreoEnviar> correosEnviar;

        //    correosEnviar = unitOfWork.CorreoEnviarRepository.GetAll();


        //    correosEnviar = correosEnviar.Where(x => x.Enviado != 1);


        //    if (correosEnviar == null)
        //    {
        //        return null;
        //    }

        //    return correosEnviar;
        //}

        //public async Task<List<CorreoEnviarConfiguracion>> GetPendingEmailsWithConfig()
        //{
        //    var emails = await unitOfWork.CorreoEnviarRepository.GetPendingEmailsWithConfigAsync();
        //    return emails ?? new List<CorreoEnviarConfiguracion>();
        //}

        //public async Task<bool> MarcarComoEnviado(Guid id)
        //{
        //    bool booOk = false;

        //    try
        //    {

        //        CorreoEnviar correoEnviar = await GetCorreoEnviar(id);

        //        if (correoEnviar == null) { return false; }

        //        correoEnviar.Enviado = 1;

        //        unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> AumentarMarcaCorreo(Guid id)
        //{
        //    bool booOk = false;

        //    try
        //    {
        //        CorreoEnviar correoEnviar = await GetCorreoEnviar(id);

        //        if (correoEnviar == null) { return false; }

        //        correoEnviar.Marca = (byte)((correoEnviar.Marca ?? 0) + 1);

        //        unitOfWork.CorreoEnviarRepository.Update(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InsertarCorreoRegistroDeVisita(CorreoRegistroVisitaDtoNuevo correoRegistroVisitaDtoNuevo, Guid UsuarioActualId, Guid empresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        booOk = false;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InsertarCorreoAvisoDePrivacidad(CorreoAvisoDePrivacidadDto correoAvisoDePrivacidadDto, Guid UsuarioActualId, Guid empresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {
        //        Visitante visitante = new Visitante();
        //        if (correoAvisoDePrivacidadDto.VisitanteId != null)
        //        {
        //            visitante = await unitOfWork.VisitanteRepository.GetById((Guid)correoAvisoDePrivacidadDto.VisitanteId);

        //        }

        //        IEnumerable<Plantilla> plantillas = unitOfWork.PlantillaRepository.GetAll();
        //        plantillas = plantillas.Where(x => x.EmpresaId == empresaId);
        //        Plantilla plantilla = plantillas.First(x => x.Identificador == new Guid("3269A899-5889-44C5-A021-19810A6C801B"));

        //        string correo = "";
        //        if (correoAvisoDePrivacidadDto.correo == "" || correoAvisoDePrivacidadDto.correo == null)
        //        {
        //            correo = visitante.Email;
        //        }
        //        else
        //        {
        //            correo = correoAvisoDePrivacidadDto.correo;
        //        }


        //        CorreoEnviar correoEnviar = new CorreoEnviar();
        //        correoEnviar.Id = Guid.NewGuid();
        //        correoEnviar.Estado = 1;
        //        correoEnviar.FechaCreacion = DateTime.Now;
        //        correoEnviar.UsuarioCreadorId = UsuarioActualId;

        //        correoEnviar.De = "proyectos@crcdemexico.com.mx";
        //        correoEnviar.Para = correo;
        //        correoEnviar.Cc = "Aviso de privacidad";
        //        correoEnviar.Mensaje = plantilla.CuerpoPlantilla;
        //        correoEnviar.Enviado = 2;
        //        correoEnviar.Marca = 1;
        //        correoEnviar.Asunto = "Aviso de privacidad";
        //        correoEnviar.EmpresaId = empresaId;

        //        await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "InsertarCorreoRegistroDeVisita";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, UsuarioActualId, empresaId);
        //        booOk = false;
        //    }

        //    return booOk;
        //}

        //public Task<string> CargarPlantillaRegistroVisita(CorreoRegistroVisitaDtoNuevo correoEnviar)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> InsertarCorreoCitaAgendada(CorreoCitaAgendadaDto datoscorreo, Guid UsuarioActualId, Guid EmpresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {
        //        Cita cita = await unitOfWork.CitaRepository.GetByIdConPermisos(datoscorreo.CitaId);

        //        if (cita == null) { return false; }

        //        Evento evento = await unitOfWork.EventoRepository.GetByCita(datoscorreo.CitaId);

        //        IEnumerable<Plantilla> plantillas = unitOfWork.PlantillaRepository.GetAll();
        //        plantillas = plantillas.Where(x => x.EmpresaId == EmpresaId);
        //        Plantilla plantilla = plantillas.FirstOrDefault(x => x.Identificador == new Guid("63A1CB97-2E58-4E1E-BBB8-20C1F0A149FF"));

        //        if (plantilla != null)
        //        {
        //            string mensaje = plantilla.CuerpoPlantilla;

        //            string correoOrganizador = "";

        //            CitaPersona citaPersona = cita.CitaPersona.FirstOrDefault();
        //            CitaColaborador citaColaborador = cita.CitaColaborador.First(x => x.EsOrganizador == 1);
        //            Colaborador colaborador = await unitOfWork.ColaboradorRepository.GetById((Guid)citaColaborador.ColaboradorId);
        //            IEnumerable<string> nombresColaboradores = new List<string>();
        //            IEnumerable<Participantes> correosParticipantes = new List<Participantes>();
        //            string NombreVisitante = "";
        //            string TelefonoVisitante = "";
        //            Guid CitaPersonaId = new Guid();

        //            foreach (var item in cita.CitaColaborador)
        //            {

        //                Colaborador colaboradorName = await unitOfWork.ColaboradorRepository.GetById((Guid)item.ColaboradorId);
        //                string nombreColaborador = colaboradorName.Nombre + " " + colaboradorName.ApellidoPaterno + " " + colaboradorName.ApellidoMaterno;
        //                if (item.EsOrganizador == 2)
        //                {
        //                    Participantes participante = new Participantes();
        //                    participante.Nombre = nombreColaborador;
        //                    participante.Correo = colaboradorName.Email;

        //                    correosParticipantes = correosParticipantes.Append(participante);
        //                }
        //                else
        //                {
        //                    correoOrganizador = colaboradorName.Email;
        //                }
        //                nombresColaboradores = nombresColaboradores.Append(nombreColaborador);
        //            }

        //            foreach (var item in cita.CitaPersona)
        //            {
        //                if (citaPersona.VisitanteId == null)
        //                {
        //                    Persona personaCita = await unitOfWork.PersonaRepository.GetById((Guid)item.PersonaId);
        //                    Participantes participante = new Participantes();
        //                    participante.Nombre = personaCita.Nombres + " " + personaCita.ApellidoPaterno + " " + personaCita.ApellidoMaterno;
        //                    participante.Correo = personaCita.CorreoElectronico;
        //                    correosParticipantes = correosParticipantes.Append(participante);

        //                }
        //                else
        //                {
        //                    Visitante visitante = await unitOfWork.VisitanteRepository.GetById((Guid)item.VisitanteId);
        //                    Participantes participante = new Participantes();
        //                    participante.Nombre = visitante.Nombre + " " + visitante.ApellidoPaterno + " " + visitante.ApellidoMaterno;
        //                    participante.Correo = visitante.Email;
        //                    correosParticipantes = correosParticipantes.Append(participante);


        //                }
        //            }

        //            mensaje = mensaje.Replace("#Asunto", cita.Asunto);
        //            mensaje = mensaje.Replace("#NombreMotivoDeVisita", cita.MotivoVisita.Nombre);
        //            mensaje = mensaje.Replace("#NombrePersonaAquienVisita", colaborador.Nombre + " " + colaborador.ApellidoPaterno + " " + colaborador.ApellidoMaterno);
        //            mensaje = mensaje.Replace("#Ubicacion", cita.Ubicacion.Nombre);
        //            mensaje = mensaje.Replace("#FechaInicio", cita.FechaInicio.ToString());
        //            mensaje = mensaje.Replace("#FechaFin", cita.FechaFin.ToString());
        //            mensaje = mensaje.Replace("#ImportanciaCita", cita.Param.Nombre);
        //            string mensajeEnviar = mensaje;

        //            foreach (var persona in cita.CitaPersona)
        //            {
        //                mensajeEnviar = mensaje;

        //                if (citaPersona.VisitanteId == null)
        //                {
        //                    Persona personaCita = await unitOfWork.PersonaRepository.GetById((Guid)persona.PersonaId);
        //                    mensajeEnviar = mensajeEnviar.Replace("#NombreVisitante", personaCita.Nombres + " " + personaCita.ApellidoPaterno + " " + personaCita.ApellidoMaterno);
        //                    mensajeEnviar = mensajeEnviar.Replace("#TelefonoDeVisitante", personaCita.TelefonoFijo);
        //                    NombreVisitante = personaCita.Nombres + " " + personaCita.ApellidoPaterno + " " + personaCita.ApellidoMaterno;
        //                    TelefonoVisitante = personaCita.TelefonoFijo;
        //                    CitaPersonaId = persona.Id;
        //                    string Qr = await GnerarQr(cita, persona.Id, NombreVisitante, TelefonoVisitante, nombresColaboradores);
        //                    string ics = await GenerarICS(cita, colaborador.Email, mensajeEnviar, correosParticipantes);
        //                    CorreoEnviar correoEnviar = new CorreoEnviar();
        //                    correoEnviar.Id = Guid.NewGuid();
        //                    correoEnviar.Estado = 1;
        //                    correoEnviar.FechaCreacion = DateTime.Now;
        //                    correoEnviar.UsuarioCreadorId = UsuarioActualId;


        //                    if (evento.ValorRetorno != null)
        //                    {
        //                        IEnumerable<EventoDtoQr> resp = JsonConvert.DeserializeObject<IEnumerable<EventoDtoQr>>(evento.ValorRetorno);

        //                        if (resp != null)
        //                        {
        //                            EventoDtoQr eventoDto = resp.FirstOrDefault(x => x.VisitanteId == persona.PersonaId);

        //                            if (eventoDto != null)
        //                            {
        //                                correoEnviar.QrAcceso = eventoDto.CredencialNumero;
        //                            }
        //                        }
        //                    }

        //                    correoEnviar.De = "proyectos@crcdemexico.com.mx";
        //                    correoEnviar.Para = personaCita.CorreoElectronico;
        //                    correoEnviar.Cc = correoOrganizador;
        //                    correoEnviar.Mensaje = mensajeEnviar;
        //                    correoEnviar.Enviado = 2;
        //                    correoEnviar.Marca = 1;
        //                    correoEnviar.Asunto = "Cita Agendada";
        //                    correoEnviar.EmpresaId = cita.EmpresaId;
        //                    correoEnviar.Qr = Qr;
        //                    correoEnviar.Ics = ics;
        //                    correoEnviar.OrganizadorCorreo = colaborador.Email;
        //                    await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);

        //                }
        //                else
        //                {
        //                    Visitante visitante = await unitOfWork.VisitanteRepository.GetById((Guid)persona.VisitanteId);
        //                    mensajeEnviar = mensajeEnviar.Replace("#NombreVisitante", visitante.Nombre + " " + visitante.ApellidoPaterno + " " + visitante.ApellidoMaterno);
        //                    mensajeEnviar = mensajeEnviar.Replace("#TelefonoDeVisitante", visitante.Telefono);
        //                    mensajeEnviar = mensajeEnviar.Replace("#DireccionDeVisitante", visitante.Calle + " " + visitante.Colonia + " " + visitante.Ciudad);
        //                    NombreVisitante = visitante.Nombre + " " + visitante.ApellidoPaterno + " " + visitante.ApellidoMaterno;
        //                    TelefonoVisitante = visitante.Telefono;
        //                    CitaPersonaId = persona.Id;
        //                    string Qr = await GnerarQr(cita, persona.Id, NombreVisitante, TelefonoVisitante, nombresColaboradores);
        //                    string ics = await GenerarICS(cita, colaborador.Email, mensajeEnviar, correosParticipantes);


        //                    CorreoEnviar correoEnviar = new CorreoEnviar();
        //                    correoEnviar.Id = Guid.NewGuid();
        //                    correoEnviar.Estado = 1;
        //                    correoEnviar.FechaCreacion = DateTime.Now;
        //                    correoEnviar.UsuarioCreadorId = UsuarioActualId;
        //                    correoEnviar.Qr = Qr;
        //                    correoEnviar.Ics = ics;

        //                    if (evento.ValorRetorno != null)
        //                    {
        //                        IEnumerable<EventoDtoQr> resp = JsonConvert.DeserializeObject<IEnumerable<EventoDtoQr>>(evento.ValorRetorno);

        //                        if (resp != null)
        //                        {
        //                            EventoDtoQr eventoDto = resp.FirstOrDefault(x => x.VisitanteId == visitante.Id);

        //                            if (eventoDto != null)
        //                            {
        //                                correoEnviar.QrAcceso = eventoDto.CredencialNumero;
        //                            }
        //                        }
        //                    }

        //                    correoEnviar.De = "proyectos@crcdemexico.com.mx";
        //                    correoEnviar.Para = visitante.Email;
        //                    correoEnviar.Cc = correoOrganizador;
        //                    correoEnviar.Mensaje = mensajeEnviar;
        //                    correoEnviar.Enviado = 2;
        //                    correoEnviar.Marca = 1;
        //                    correoEnviar.Asunto = "Cita Agendada";
        //                    correoEnviar.EmpresaId = cita.EmpresaId;
        //                    correoEnviar.OrganizadorCorreo = colaborador.Email;

        //                    await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);

        //                }
        //            }

        //            foreach (var colaboradorCita in cita.CitaColaborador)
        //            {
        //                mensajeEnviar = mensaje;
        //                CorreoEnviar correoEnviar = new CorreoEnviar();
        //                mensajeEnviar = mensajeEnviar.Replace("#NombreVisitante", NombreVisitante);
        //                mensajeEnviar = mensajeEnviar.Replace("#TelefonoDeVisitante", TelefonoVisitante);
        //                string Qr = await GnerarQr(cita, CitaPersonaId, NombreVisitante, TelefonoVisitante, nombresColaboradores);
        //                string ics = await GenerarICS(cita, colaborador.Email, mensajeEnviar, correosParticipantes);

        //                correoEnviar.Id = Guid.NewGuid();
        //                correoEnviar.Estado = 1;
        //                correoEnviar.FechaCreacion = DateTime.Now;
        //                correoEnviar.UsuarioCreadorId = UsuarioActualId;
        //                correoEnviar.Qr = Qr;
        //                correoEnviar.Ics = ics;
        //                correoEnviar.OrganizadorCorreo = colaborador.Email;

        //                correoEnviar.De = "proyectos@crcdemexico.com.mx";
        //                correoEnviar.Para = colaboradorCita.Colaborador.Email;
        //                correoEnviar.Cc = correoOrganizador;
        //                correoEnviar.Mensaje = mensajeEnviar;
        //                correoEnviar.Enviado = 2;
        //                correoEnviar.Marca = 1;
        //                correoEnviar.Asunto = "Cita Agendada";
        //                correoEnviar.EmpresaId = cita.EmpresaId;

        //                await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);
        //            }

        //            await unitOfWork.SaveChangesAsync();

        //        }


        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "InsertarCorreoCitaAgendada";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, UsuarioActualId, EmpresaId);
        //        booOk = false;
        //    }


        //    return booOk;
        //}

        //public async Task<string> CargarPlantillaCitaAgendada(CorreoCitaAgendaDtoPlantilla correoEnviar, Guid EmpresaId)
        //{
        //    string mensaje = "";

        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //        mensaje = null;
        //    }

        //    return mensaje;
        //}

        //public async Task<string> GnerarQr(Cita cita, Guid CitaPersonaId, string NombreVisitante, string TelefonoVisitante, IEnumerable<string> NombresColaboradores)
        //{
        //    try
        //    {
        //        QrNuevo qr = new QrNuevo();
        //        qr.CitaId = cita.Id;
        //        qr.CitaPersonaId = CitaPersonaId;
        //        qr.Nombre = NombreVisitante;
        //        qr.Telefono = TelefonoVisitante;
        //        qr.PrioridadId = cita.Prioridad;
        //        qr.Prioridad = cita.Param.Nombre;
        //        qr.MotivoVisitaId = cita.MotivoVisitaId;
        //        qr.MotivoDeVisita = cita.MotivoVisita.Nombre;
        //        qr.UbicacionId = cita.UbicacionId;
        //        qr.Ubicacion = cita.Ubicacion.Nombre;
        //        qr.Colaboradores = NombresColaboradores;
        //        string datos_qr = JsonConvert.SerializeObject(qr);

        //        return datos_qr;
        //    }
        //    catch (Exception)
        //    {

        //        return null;
        //    }
        //}

        //public async Task<string> GenerarICS(Cita cita, string Organizador, string Mensaje, IEnumerable<Participantes> participantes)
        //{
        //    try
        //    {
        //        Ics ics = new Ics();
        //        ics.Ubicacion = cita.Ubicacion.Nombre;
        //        ics.MotivoDeVisita = cita.Asunto;
        //        ics.Organizador = Organizador;
        //        ics.FechaInicio = cita.FechaInicio;
        //        ics.FechaFin = cita.FechaFin;
        //        ics.Mensaje = Mensaje;
        //        ics.participantes = participantes;
        //        string datos_qr = JsonConvert.SerializeObject(ics);

        //        return datos_qr;
        //    }
        //    catch (Exception)
        //    {

        //        return null;
        //    }
        //}

        public async Task<bool> SendUserEmail(CorreoEnviarUsuario data, Guid currentUserId, Guid companyId)
        {
            bool booOk = false;

            try
            {
                string mensaje = GetTemplateBody();

                mensaje = mensaje.Replace("#Nombre", data.Nombre);
                mensaje = mensaje.Replace("#Usuario", data.Correo);
                mensaje = mensaje.Replace("#Contrasena", data.Contrasena);

                CorreoEnviar correoEnviar = new CorreoEnviar();
                correoEnviar.Id = Guid.NewGuid();
                correoEnviar.Estado = 1;
                correoEnviar.FechaCreacion = DateTime.Now;
                correoEnviar.UsuarioCreadorId = currentUserId;

                correoEnviar.De = "proyectos@crcdemexico.com.mx";
                correoEnviar.Para = data.Correo;
                correoEnviar.Cc = null;
                correoEnviar.Mensaje = mensaje;
                correoEnviar.Enviado = 2;
                correoEnviar.Marca = 1;
                correoEnviar.Asunto = "Usuario Portal WebVisitsMobile";
                correoEnviar.EmpresaClienteId = companyId;

                await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);
                await unitOfWork.SaveChangesAsync();

                booOk = true;
            }
            catch (Exception ex)
            {
                booOk = false;
            }

            return booOk;
        }

        public string GetTemplateBody()
        {
            return "Hola,\r\n\r\n    ¡Bienvenido(a) al Portal WebVisitsMobile!\r\n\r\n    Nos alegra tenerte con nosotros. A continuación, encontrarás tus credenciales para acceder al portal:\r\n\r\n    Usuario: #Usuario\r\n    Contraseña: #Contrasena\r\n    Link: http://localhost:4200/web-visits-mobile/#/sign-in\r\n\r\n    Recuerda cambiar tu contraseña después de iniciar sesión por primera vez para mantener tu cuenta segura.\r\n\r\n    Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.\r\n\r\n    Saludos cordiales.\r\n";
        }

        //public async Task<bool> InsertarCorreoCitaNoAutorizada(CorreoCitaAgendadaDto datos, Guid UsuarioActualId, Guid EmpresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {


        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        booOk = false;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InsertarCorreoCitaAutorizada(CorreoCitaAgendadaDto datoscorreo, Guid UsuarioActualId, Guid EmpresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {


        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        booOk = false;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InsertarCorreoVisitantesConTiempoExwdido(string correo, Guid UsuarioActualId, Guid EmpresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {



        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        booOk = false;
        //    }

        //    return booOk;
        //}

        //public async Task<bool> InsertarCorreoAltaMovil(CorreoAltaUsuario datos, Guid UsuarioActualId, Guid EmpresaId)
        //{
        //    bool booOk = false;

        //    try
        //    {

        //        IEnumerable<Plantilla> plantillas = unitOfWork.PlantillaRepository.GetAll();
        //        plantillas = plantillas.Where(x => x.EmpresaId == EmpresaId);
        //        Plantilla plantilla = plantillas.First(x => x.Identificador == new Guid("AAAE9603-9348-46B1-9550-5263DDEBDDCD"));
        //        string mensaje = plantilla.CuerpoPlantilla;

        //        mensaje = mensaje.Replace("#Nombre", datos.Nombre);
        //        mensaje = mensaje.Replace("#Usuario", datos.Correo);
        //        mensaje = mensaje.Replace("#Contrasena", datos.Contrasena);


        //        CorreoEnviar correoEnviar = new CorreoEnviar();
        //        correoEnviar.Id = Guid.NewGuid();
        //        correoEnviar.Estado = 1;
        //        correoEnviar.FechaCreacion = DateTime.Now;
        //        correoEnviar.UsuarioCreadorId = UsuarioActualId;

        //        correoEnviar.De = "proyectos@crcdemexico.com.mx";
        //        correoEnviar.Para = datos.Correo;
        //        correoEnviar.Cc = "Visitantes con tiempo extendido";
        //        correoEnviar.Mensaje = mensaje;
        //        correoEnviar.Enviado = 2;
        //        correoEnviar.Marca = 1;
        //        correoEnviar.Asunto = "Visitantes con tiempo extendido";
        //        correoEnviar.EmpresaId = EmpresaId;

        //        await unitOfWork.CorreoEnviarRepository.Add(correoEnviar);

        //        await unitOfWork.SaveChangesAsync();

        //        booOk = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logE logE = new logE();
        //        logE.Mensaje = ex.Message;
        //        logE.Controlador = "CorreoEnviarServices";
        //        logE.Metodo = "InsertarCorreoAltaMovil";
        //        logE.AplicacionLogId = new Guid("892603D1-B81D-4782-8353-F0E8CC0ECB75");

        //        logService.InsertlogE(logE, UsuarioActualId, EmpresaId);
        //        booOk = false;
        //    }

        //    return booOk;
        //}
    }
}