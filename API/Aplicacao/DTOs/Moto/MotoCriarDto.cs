﻿using API.Domain.Persistence;
using System.ComponentModel.DataAnnotations;

namespace API.Application.DTOs.Moto;

public class MotoCriarDto
{
    // TODO: Remover validações do DTO e manter em DOMAIN
    [Required(ErrorMessage = "Placa não pode estar vazia.")]
    [StringLength(7, MinimumLength = 6,ErrorMessage = "Placa deve ter no mínmo 6 e no máximo 7 caracteres.")] // Considerando apenas Brasil
    public string Placa { get; set; }

    [Required(ErrorMessage = "Modelo não pode estar vazio.")]
    public string Modelo { get; set; } // string para facilitar a conversão do enum

    [Required(ErrorMessage = "Filial não pode estar vazia.")]
    public int IdFilial { get; set; } // Considerando que o idFilial é um inteiro que representa a filial

    public MotoCriarDto(string placa, string modelo, int idFilial)
    {
        Placa = placa;
        // TODO: Remover esse ToUpper ou jogar lógica para classe de DOMAIN
        Modelo = modelo.ToUpper();
        IdFilial = idFilial;
    }
}
