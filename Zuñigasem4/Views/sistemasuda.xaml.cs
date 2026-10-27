using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Zuñigasem4.Views;

/// <summary>
/// Vista principal del Sistema de Calificaciones del Instituto Sudamericano.
/// Gestiona la validación de notas, cálculos ponderados y presentación de resultados.
/// </summary>
public partial class sistemasuda : ContentPage
{
    public sistemasuda()
    {
        InitializeComponent();

        // Establecer la fecha actual por defecto
        dpFecha.Date = DateTime.Today;
    }

    /// <summary>
    /// Evento disparado cada vez que el usuario modifica el texto de alguna nota.
    /// Realiza el cálculo automático en tiempo real si los valores son válidos.
    /// </summary>
    private void OnNotaTextChanged(object? sender, TextChangedEventArgs e)
    {
        ActualizarCalculosEnTiempoReal();
    }

    /// <summary>
    /// Evento disparado al cambiar la selección del estudiante.
    /// </summary>
    private void OnPickerEstudianteChanged(object? sender, EventArgs e)
    {
        // Se puede sincronizar o verificar el estudiante seleccionado
    }

    /// <summary>
    /// Realiza el cálculo automático de los subtotales de cada parcial y, si ambos están listos,
    /// calcula la nota final y el estado académico preliminar.
    /// </summary>
    private void ActualizarCalculosEnTiempoReal()
    {
        bool p1Listo = false;
        double notaP1 = 0.0;

        // Cálculo Parcial 1: (Seguimiento1 * 0.3) + (Examen1 * 0.2)
        if (TryParseNota(txtSeguimiento1.Text, out double seg1) && seg1 >= 0 && seg1 <= 10 &&
            TryParseNota(txtExamen1.Text, out double exa1) && exa1 >= 0 && exa1 <= 10)
        {
            notaP1 = (seg1 * 0.3) + (exa1 * 0.2);
            lblParcial1.Text = notaP1.ToString("F2", CultureInfo.InvariantCulture);
            lblParcial1Badge.Text = $"Subtotal: {notaP1.ToString("F2", CultureInfo.InvariantCulture)} / 5.00";
            p1Listo = true;
        }
        else
        {
            lblParcial1.Text = "0.00";
            lblParcial1Badge.Text = "Subtotal: 0.00 / 5.00";
        }

        bool p2Listo = false;
        double notaP2 = 0.0;

        // Cálculo Parcial 2: (Seguimiento2 * 0.3) + (Examen2 * 0.2)
        if (TryParseNota(txtSeguimiento2.Text, out double seg2) && seg2 >= 0 && seg2 <= 10 &&
            TryParseNota(txtExamen2.Text, out double exa2) && exa2 >= 0 && exa2 <= 10)
        {
            notaP2 = (seg2 * 0.3) + (exa2 * 0.2);
            lblParcial2.Text = notaP2.ToString("F2", CultureInfo.InvariantCulture);
            lblParcial2Badge.Text = $"Subtotal: {notaP2.ToString("F2", CultureInfo.InvariantCulture)} / 5.00";
            p2Listo = true;
        }
        else
        {
            lblParcial2.Text = "0.00";
            lblParcial2Badge.Text = "Subtotal: 0.00 / 5.00";
        }

        // Si ambos parciales son válidos, actualizar nota final y estado en pantalla
        if (p1Listo && p2Listo)
        {
            double notaFinal = notaP1 + notaP2;
            lblFinal.Text = $"{notaFinal.ToString("F2", CultureInfo.InvariantCulture)} / 10.00";
            ActualizarEstadoUI(notaFinal);
        }
        else
        {
            lblFinal.Text = "0.00 / 10.00";
            lblEstado.Text = "Sin Calcular";
            lblEstado.TextColor = Color.FromArgb("#475569");
            borderEstado.BackgroundColor = Color.FromArgb("#E2E8F0");
        }
    }

    /// <summary>
    /// Aplica estilos visuales y colores al estado académico del estudiante.
    /// </summary>
    private string ActualizarEstadoUI(double notaFinal)
    {
        string estado;

        if (notaFinal >= 7.0)
        {
            estado = "Aprobado";
            lblEstado.Text = "Aprobado";
            lblEstado.TextColor = Color.FromArgb("#15803D"); // Verde profesional
            borderEstado.BackgroundColor = Color.FromArgb("#DCFCE7"); // Verde claro
        }
        else if (notaFinal >= 5.0)
        {
            estado = "Complementario";
            lblEstado.Text = "Complementario";
            lblEstado.TextColor = Color.FromArgb("#B45309"); // Ámbar / naranja
            borderEstado.BackgroundColor = Color.FromArgb("#FEF3C7"); // Amarillo claro
        }
        else
        {
            estado = "Reprobado";
            lblEstado.Text = "Reprobado";
            lblEstado.TextColor = Color.FromArgb("#B91C1C"); // Rojo
            borderEstado.BackgroundColor = Color.FromArgb("#FEE2E2"); // Rojo claro
        }

        return estado;
    }

    /// <summary>
    /// Manejador del botón principal: valida campos, calcula resultados y muestra el DisplayAlert.
    /// </summary>
    private async void Calcular_Clicked(object? sender, EventArgs e)
    {
#pragma warning disable CS0618 // Desactivar advertencia de DisplayAlert obsoleto para compatibilidad
        // 1. Validación de Estudiante
        if (pickerEstudiante.SelectedIndex < 0 || pickerEstudiante.SelectedItem == null)
        {
            await DisplayAlert("Validación Requerida", "Por favor seleccione un estudiante de la lista antes de continuar.", "Aceptar");
            pickerEstudiante.Focus();
            return;
        }

        // 2. Validación de Nota Seguimiento 1
        if (string.IsNullOrWhiteSpace(txtSeguimiento1.Text))
        {
            await DisplayAlert("Campo Vacío", "Por favor ingrese la Nota de Seguimiento 1.", "Aceptar");
            txtSeguimiento1.Focus();
            return;
        }
        if (!TryParseNota(txtSeguimiento1.Text, out double seg1))
        {
            await DisplayAlert("Formato Incorrecto", "La Nota de Seguimiento 1 debe ser un valor numérico.", "Aceptar");
            txtSeguimiento1.Focus();
            return;
        }
        if (seg1 < 0.0 || seg1 > 10.0)
        {
            await DisplayAlert("Rango Inválido", "La Nota de Seguimiento 1 debe estar comprendida entre 0.00 y 10.00.", "Aceptar");
            txtSeguimiento1.Focus();
            return;
        }

        // 3. Validación de Examen 1
        if (string.IsNullOrWhiteSpace(txtExamen1.Text))
        {
            await DisplayAlert("Campo Vacío", "Por favor ingrese la nota del Examen 1.", "Aceptar");
            txtExamen1.Focus();
            return;
        }
        if (!TryParseNota(txtExamen1.Text, out double exa1))
        {
            await DisplayAlert("Formato Incorrecto", "La nota del Examen 1 debe ser un valor numérico.", "Aceptar");
            txtExamen1.Focus();
            return;
        }
        if (exa1 < 0.0 || exa1 > 10.0)
        {
            await DisplayAlert("Rango Inválido", "La nota del Examen 1 debe estar comprendida entre 0.00 y 10.00.", "Aceptar");
            txtExamen1.Focus();
            return;
        }

        // 4. Validación de Nota Seguimiento 2
        if (string.IsNullOrWhiteSpace(txtSeguimiento2.Text))
        {
            await DisplayAlert("Campo Vacío", "Por favor ingrese la Nota de Seguimiento 2.", "Aceptar");
            txtSeguimiento2.Focus();
            return;
        }
        if (!TryParseNota(txtSeguimiento2.Text, out double seg2))
        {
            await DisplayAlert("Formato Incorrecto", "La Nota de Seguimiento 2 debe ser un valor numérico.", "Aceptar");
            txtSeguimiento2.Focus();
            return;
        }
        if (seg2 < 0.0 || seg2 > 10.0)
        {
            await DisplayAlert("Rango Inválido", "La Nota de Seguimiento 2 debe estar comprendida entre 0.00 y 10.00.", "Aceptar");
            txtSeguimiento2.Focus();
            return;
        }

        // 5. Validación de Examen 2
        if (string.IsNullOrWhiteSpace(txtExamen2.Text))
        {
            await DisplayAlert("Campo Vacío", "Por favor ingrese la nota del Examen 2.", "Aceptar");
            txtExamen2.Focus();
            return;
        }
        if (!TryParseNota(txtExamen2.Text, out double exa2))
        {
            await DisplayAlert("Formato Incorrecto", "La nota del Examen 2 debe ser un valor numérico.", "Aceptar");
            txtExamen2.Focus();
            return;
        }
        if (exa2 < 0.0 || exa2 > 10.0)
        {
            await DisplayAlert("Rango Inválido", "La nota del Examen 2 debe estar comprendida entre 0.00 y 10.00.", "Aceptar");
            txtExamen2.Focus();
            return;
        }

        // CÁLCULOS
        // Parcial 1: (NotaSeguimiento1 × 0.3) + (Examen1 × 0.2)
        double notaP1 = (seg1 * 0.3) + (exa1 * 0.2);

        // Parcial 2: (NotaSeguimiento2 × 0.3) + (Examen2 × 0.2)
        double notaP2 = (seg2 * 0.3) + (exa2 * 0.2);

        // Nota Final: Nota Parcial 1 + Nota Parcial 2
        double notaFinal = notaP1 + notaP2;

        // Determinar Estado y actualizar UI
        string estado = ActualizarEstadoUI(notaFinal);

        // Actualizar etiquetas en la vista
        lblParcial1.Text = notaP1.ToString("F2", CultureInfo.InvariantCulture);
        lblParcial1Badge.Text = $"Subtotal: {notaP1.ToString("F2", CultureInfo.InvariantCulture)} / 5.00";

        lblParcial2.Text = notaP2.ToString("F2", CultureInfo.InvariantCulture);
        lblParcial2Badge.Text = $"Subtotal: {notaP2.ToString("F2", CultureInfo.InvariantCulture)} / 5.00";

        lblFinal.Text = $"{notaFinal.ToString("F2", CultureInfo.InvariantCulture)} / 10.00";

        // Obtener datos del estudiante y fecha formateada
        string nombreEstudiante = pickerEstudiante.SelectedItem?.ToString() ?? string.Empty;
        DateTime fecha = dpFecha.Date ?? DateTime.Today;
        string fechaFormateada = fecha.ToString("dd/MM/yyyy");

        // Construir el mensaje para el DisplayAlert exactamente con los campos requeridos
        string mensaje =
            $"Nombre: {nombreEstudiante}\n" +
            $"Fecha: {fechaFormateada}\n" +
            $"Nota Parcial 1: {notaP1.ToString("F2", CultureInfo.InvariantCulture)}\n" +
            $"Nota Parcial 2: {notaP2.ToString("F2", CultureInfo.InvariantCulture)}\n" +
            $"Nota Final: {notaFinal.ToString("F2", CultureInfo.InvariantCulture)}\n" +
            $"Estado: {estado}";

        // Mostrar alerta con los resultados del estudiante
        await DisplayAlert("Reporte de Calificaciones", mensaje, "Aceptar");
#pragma warning restore CS0618
    }

    /// <summary>
    /// Restablece los campos del formulario para ingresar nuevos datos.
    /// </summary>
    private void Limpiar_Clicked(object? sender, EventArgs e)
    {
        pickerEstudiante.SelectedIndex = -1;
        dpFecha.Date = DateTime.Today;

        txtSeguimiento1.Text = string.Empty;
        txtExamen1.Text = string.Empty;
        txtSeguimiento2.Text = string.Empty;
        txtExamen2.Text = string.Empty;

        lblParcial1.Text = "0.00";
        lblParcial1Badge.Text = "Subtotal: 0.00 / 5.00";

        lblParcial2.Text = "0.00";
        lblParcial2Badge.Text = "Subtotal: 0.00 / 5.00";

        lblFinal.Text = "0.00 / 10.00";
        lblEstado.Text = "Sin Calcular";
        lblEstado.TextColor = Color.FromArgb("#475569");
        borderEstado.BackgroundColor = Color.FromArgb("#E2E8F0");
    }

    /// <summary>
    /// Intenta parsear una cadena a número decimal de manera robusta,
    /// permitiendo tanto comas como puntos decimales.
    /// </summary>
    private static bool TryParseNota(string? input, out double valor)
    {
        valor = 0.0;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string normalized = input.Trim().Replace(',', '.');

        return double.TryParse(
            normalized,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out valor);
    }
}