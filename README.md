# API Restful para Contação de Seguro
O Desafio é desenvolver uma Web API RESTful para cálculo, armazenamento e consulta de cotações de seguros. A aplicação será consumida por empresas parceiras e deve incluir operações CRUD (criar, alterar, listar, detalhar e excluir) para cotações, beneficiários e coberturas, além de regras de negócio específicas para cálculos de prêmios e validações.

## Tecnologias Exigidas no Desafio
- Framework: .NET Core 3.1 ou superior
- Banco de Dados: MSSQL (Developer/Express ou superior)
- Acesso a Dados: Entity Framework ou Dapper
- Testes Unitários: XUnit ou MSTest
- Restrições: Apenas ferramentas gratuitas (sem plugins pagos).
  
## Banco de Dados - SQL SERVER ![Static Badge](https://img.shields.io/badge/Docker-blue?logo=Docker)
Durante o desenvolvimento optei em utilizar o SQL SERVER 2022 via Docker
Comando para levantar a imagem do container:

> docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SuaSenha" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

A Documentação da microsoft fala sobre o SA_PASSWORD ter sido preterida, e hoje recomendam o MSSQL_SA_PASSWORD, mas acabei usando o SA_PASSWORD 
e funcionou bem.
Link da documentação sobre SQL SERVER no Docker:  (https://learn.microsoft.com/pt-br/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&tabs=cli&pivots=cs1-bash)
## Script de Criação de Tabelas
Foi criado uma base chamada OmniBeesCreativeDB e um script para as criações das tabelas auxiliares e principais. 

As tabelas criadas foram baseadas no que pedia o desafio, necessitando um pequeno ajustes nos nomes de algumas tabelas auxiliares, de *Key*, optei para o *Id*, por se tratar de uma palavra reservada do SQL.
  ```sql
  -- Modelo Entidade-Relacionamento (MER) para o teste técnico Omnibees
  
  -- Tabelas principais e auxiliares com chaves primárias (PK) e estrangeiras (FK)
  
  
  -- Tabela: Cotacao (armazena as cotações de seguro)
  
  CREATE TABLE Cotacao (
      Id INT PRIMARY KEY IDENTITY(1,1),           -- Chave primária autoincremental
      IdProduto INT NOT NULL,                     -- FK para Produto
      DataCriacao DATE NOT NULL,                  -- Data de criação
      DataAtualizacao DATE NOT NULL,              -- Data da última atualização
      IdParceiro INT NOT NULL,                    -- FK para Parceiro
      NomeSegurado VARCHAR(100) NOT NULL,         -- Nome do segurado
      DDD INT,                                    -- Opcional
      Telefone INT,                               -- Opcional
      Endereco VARCHAR(255) NOT NULL,             -- Endereço completo
      CEP VARCHAR(8) NOT NULL,                    -- CEP (sem hífen)
      Documento VARCHAR(20) NOT NULL,             -- CPF ou outro documento
      Nascimento DATE NOT NULL,                   -- Data de nascimento
      Premio DECIMAL(18,2) NOT NULL,              -- Valor do prêmio
      ImportanciaSegurada DECIMAL(18,2) NOT NULL, -- Valor da indenização
      CONSTRAINT FK_Cotacao_Produto FOREIGN KEY (IdProduto) REFERENCES Produto(Id),
      CONSTRAINT FK_Cotacao_Parceiro FOREIGN KEY (IdParceiro) REFERENCES Parceiro(Id)
  );
  
  -- Tabela: CotacaoBeneficiario (beneficiários de uma cotação)
  CREATE TABLE CotacaoBeneficiario (
      Id INT PRIMARY KEY IDENTITY(1,1),           -- Chave primária autoincremental
      IdCotacao INT NOT NULL,                     -- FK para Cotacao
      Nome VARCHAR(100) NOT NULL,                 -- Nome do beneficiário
      Percentual DECIMAL(5,2) NOT NULL,           -- Percentual de participação (máx 100.00)
      IdParentesco INT NOT NULL,                  -- FK para Tipo de Parentesco
      CONSTRAINT FK_Beneficiario_Cotacao FOREIGN KEY (IdCotacao) REFERENCES Cotacao(Id),
      CONSTRAINT FK_Beneficiario_Parentesco FOREIGN KEY (IdParentesco) REFERENCES TipoParentesco(Id)
  );
  
  -- Tabela: CotacaoCobertura (coberturas de uma cotação)
  CREATE TABLE CotacaoCobertura (
      Id INT PRIMARY KEY IDENTITY(1,1),           -- Chave primária autoincremental
      IdCotacao INT NOT NULL,                     -- FK para Cotacao
      IdCobertura INT NOT NULL,                   -- FK para Cobertura
      ValorDesconto DECIMAL(18,2),                -- Desconto (nulo para coberturas adicionais)
      ValorAgravo DECIMAL(18,2),                  -- Agravo (nulo para coberturas adicionais)
      ValorTotal DECIMAL(18,2) NOT NULL,          -- Valor final da cobertura
      CONSTRAINT FK_Cobertura_Cotacao FOREIGN KEY (IdCotacao) REFERENCES Cotacao(Id),
      CONSTRAINT FK_Cobertura_Cobertura FOREIGN KEY (IdCobertura) REFERENCES Cobertura(Id)
  );
  
  -- Tabela Auxiliar: Produto
  CREATE TABLE Produto (
      Id INT PRIMARY KEY,                        -- Chave primária fixa
      Description VARCHAR(50) NOT NULL,           -- Descrição do produto
      BaseValue DECIMAL(18,2) NOT NULL,           -- Valor base
      Limit DECIMAL(18,2) NOT NULL                -- Limite máximo da IS
  );
  
  -- Tabela Auxiliar: Parceiro
  CREATE TABLE Parceiro (
      Id INT PRIMARY KEY,                        -- Chave primária fixa
      Description VARCHAR(50) NOT NULL,           -- Nome do parceiro
      Secret VARCHAR(10) NOT NULL                 -- Chave secreta
  );
  
  -- Tabela Auxiliar: TipoParentesco
  CREATE TABLE TipoParentesco (
      Id INT PRIMARY KEY,                        -- Chave primária fixa
      Description VARCHAR(20) NOT NULL            -- Descrição do parentesco
  );
  
  -- Tabela Auxiliar: Cobertura
  CREATE TABLE Cobertura (
      Id INT PRIMARY KEY,                        -- Chave primária fixa
      Description VARCHAR(50) NOT NULL,           -- Descrição da cobertura
      Type VARCHAR(20) NOT NULL,                  -- Tipo (Básica ou Adicional)
      Value DECIMAL(18,2) NOT NULL                -- Valor base da cobertura
  );
  
  -- Tabela Auxiliar: FaixaIdade
  CREATE TABLE FaixaIdade (
      Id INT PRIMARY KEY,                        -- Chave primária fixa
      Description VARCHAR(20) NOT NULL,           -- Descrição da faixa
      Desconto DECIMAL(5,2) NOT NULL,             -- Percentual de desconto
      Agravo DECIMAL(5,2) NOT NULL                -- Percentual de agravo
  );

```
### Script para popular tabelas Auxiliares:
```sql
-- Inserção de dados nas tabelas auxiliares

INSERT INTO Produto (Id, Description, BaseValue, Limit) VALUES
(1, 'Vida Starter', 10.00, 10000.00),
(2, 'Vida AP+', 12.50, 20000.00),
(3, 'Vida Plus Master', 20.00, 100000.00),
(4, 'Vida Galaxy Membership', 4500.00, 10000000.00);

INSERT INTO Parceiro (Id, Description, Secret) VALUES
(1, 'Lojas Jackellino', 'XPTO2'),
(2, 'Rede Cusco de La Rocha', 'IDKFA'),
(3, '2 Irmãos Global Membership Traders', 'IDDQD');

INSERT INTO TipoParentesco (Id, Description) VALUES
(1, 'Mãe'),
(2, 'Pai'),
(3, 'Cônjuge'),
(4, 'Filho(a)'),
(5, 'Outros');

INSERT INTO Cobertura (Id, Description, Type, Value) VALUES
(1, 'Morte Acidental', 'Básica', 40.00),
(2, 'Morte Qualquer Causa', 'Básica', 36.50),
(3, 'Invalidez Parcial ou Total', 'Básica', 28.95),
(4, 'Assistência Funeral', 'Adicional', 18.96),
(5, 'Assistência Odontológica', 'Adicional', 12.55),
(6, 'Assistência PET', 'Adicional', 0.00); -- Valor não especificado, ajuste se necessário

INSERT INTO FaixaIdade (Id, Description, Desconto, Agravo) VALUES
(1, '6 a 18 Anos', 20.00, 0.00),
(2, '19 a 25 Anos', 10.00, 0.00),
(3, '26 a 35 Anos', 0.00, 0.00),
(4, '36 a 42 Anos', 0.00, 20.00),
(5, '42 a 65 Anos', 0.00, 40.00);
```
### Configurar ConnectionStrings
Abra o *appsettings.json* e atualize a connection string com as informações do seu SQL Server.

Garanta que o *TrustServerCertificate=true;* esteja na sua connection para evitar problemas.
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=OmniBeesCreativeDB;Encrypt=true;TrustServerCertificate=true;User Id=seu_user;Password=seu_pass;"
},
```
### Rodar o projeto:

Após clonar o repositorio, caminhe até o local do projeto rode o comando para garantir que todas as bibliotecas necessárias instalará:
```
dotnet restore
```

