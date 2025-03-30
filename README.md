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
Foi criado um script para as criações das tabelas auxiliares e principais. 
As tabelas criadas foram baseadas no que pedia o desafio, necessitando um pequeno ajustes nos nomes de algumas tabelas auxiliares, de *Key*, optei para o *Id*, por se tratar de uma palavra reservada do SQL.
