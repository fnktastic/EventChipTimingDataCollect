using AutoMapper;
using ReaderDataCollector.AtwService;
using ReaderDataCollector.Data.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitialiseMapper();
        }

        public static void InitialiseMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AtwService.Read, Data.Model.Read>();
                cfg.CreateMap<Data.Model.Read, AtwService.Read>();

                cfg.CreateMap<AtwService.Reading, Data.Model.Reading>();
                cfg.CreateMap<Data.Model.Reading, AtwService.Reading>();

                cfg.CreateMap<AtwService.Reader, Data.Model.Reader>();
                cfg.CreateMap<Data.Model.Reader, AtwService.Reader>();

                cfg.CreateMap<AtwService.Race, Data.Model.Race>();
                cfg.CreateMap<Data.Model.Race, AtwService.Race>();
            });
        }
    }
}
