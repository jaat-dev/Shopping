﻿using System.ComponentModel.DataAnnotations;

namespace Shopping.Models;

public class StateViewModel
{
    public int Id { get; set; }

    [Display(Name = "Departamento/Estado")]
    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string? Name { get; set; }

    public int CountryId { get; set; }
}
