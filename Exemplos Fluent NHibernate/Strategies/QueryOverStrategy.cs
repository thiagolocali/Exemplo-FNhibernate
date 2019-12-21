﻿using System;
using System.Linq;
using BgmRodotec.Treinamento.NHibernate.Configuration;
using BgmRodotec.Treinamento.NHibernate.Models;
using NHibernate.Transform;

namespace BgmRodotec.Treinamento.NHibernate.Strategies
{
    public class QueryOverStrategy
    {
        public static void LazyLoad(long id)
        {
            using (var session = ConfigurationNHiberante.CreateSession())
            {
              
                var query = session.QueryOver<Pessoa>()
                    .Where(pessoa => pessoa.Id == id);
                Console.WriteLine(query.List().First());
            }
        }

        
        public static void Batch(long id)
        {
            using (var session = ConfigurationNHiberante.CreateSession())
            {
                Telefone telefoneAlias = null;
                TipoTelefone tipoTelefoneAlias = null;

                var query = session.QueryOver<Pessoa>()
                    .Where(pessoa => pessoa.Id == id)
                    .Future();

                session.QueryOver<Pessoa>()
                    .Fetch(pessoa => pessoa.Enderecos).Eager
                    .Future();

                session.QueryOver<Pessoa>()
                    .Left.JoinAlias(pessoa => pessoa.Telefones, () => telefoneAlias)
                    .Left.JoinAlias(() => telefoneAlias.TipoTelefone, () => tipoTelefoneAlias)
                    .Future();


                Console.WriteLine(query.ToList().First());
            }
        }

        public static void EagerLoad(long id)
        {
            using (var session = ConfigurationNHiberante.CreateSession())
            {
                Telefone telefoneAlias = null;
                TipoTelefone tipoTelefoneAlias = null;

                var query = session.QueryOver<Pessoa>()
                    .Fetch(pessoa => pessoa.Enderecos).Eager
                    .Left.JoinAlias(pessoa => pessoa.Telefones, () => telefoneAlias)
                    .Left.JoinAlias(() => telefoneAlias.TipoTelefone, () => tipoTelefoneAlias)
                    .Where(pessoa => pessoa.Id == id)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List().First();

                Console.WriteLine(query);
            }
        }
    }
}