﻿using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Domain.Persistence;

public class Moto
{
    public int Id { get; private set; }
    public string Placa { get; private set; }

    // Modelo
    public ModeloMoto Modelo { get; private set; } // Chave estrangeira para a tabela de Modelos, pode receber o enum ModeloMoto ou o idModelo

    // Filial
    public int IdFilial { get; private set; } // Chave estrangeira para a tabela de Filiais
    public Filial Filial { get; private set; } // Navegação para a entidade Filial

    // Construtor da classe Moto
    public Moto(string placa, string nomeModelo, int idFilial, Filial filial)
    {
        DefinirPlaca(placa);
        DefinirModelo(nomeModelo);// Converte o nomeModelo para o enum ModeloMoto
        this.IdFilial = idFilial; // Atribui o idFilial
        this.Filial = filial; // Atribui a filial
    }

    public Moto() { } // Construtor padrão para o Entity Framework

    // Métodos para alterar a placa e o modelo da moto
    
    private void DefinirPlaca(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            throw new ExcecaoDominio("Placa não pode ser nula ou vazia.", nameof(placa));
        }
        else if (placa.Length < 6 || placa.Length > 7)
        {
            throw new ArgumentException("Placa deve ter no mínimo 6 e no máximo 7 caracteres.", nameof(placa));
        }

        this.Placa = placa.ToUpper(); // Converte a placa para letras maiúsculas
    }
    private void DefinirModelo(string nomeModelo)
    {
        var modeloUpper = nomeModelo.ToUpper();
        if (!Enum.IsDefined(typeof(ModeloMoto), modeloUpper))
        {
            throw new ArgumentOutOfRangeException(nameof(nomeModelo), "Modelo inválido.");
        }
        Modelo = Enum.Parse<ModeloMoto>(modeloUpper, ignoreCase: true); // Converte o idModelo para o enum ModeloMoto
    }

    // Métodos públicos para alterar a placa e o modelo da moto
    public void AlterarPlaca(string novaPlaca)
    {
        DefinirPlaca(novaPlaca); // Chama o método para validar e atribuir a nova placa
    }

    public void AlterarModelo(string novoModelo)
    {
        DefinirModelo(novoModelo); // Chama o método para validar e atribuir o novo modelo
    }

    public void AlterarFilial(int novoIdFilial,Filial novaFilial)
    {
        this.IdFilial = novoIdFilial; // Atribui o novo idFilial
        this.Filial = novaFilial; // Atribui a nova filial
    }
}