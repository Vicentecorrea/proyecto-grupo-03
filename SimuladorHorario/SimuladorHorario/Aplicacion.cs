﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorHorario
{

    public enum TipoEvento { PRBA, CLAS, LABT, AYUD, EXTRAP, PERS }
    public enum Especialidad {ING, IOC, ICE, ICC, ICI, ICA }
    public enum TipoCurso { Curricular, Extra}
    public enum Concentracion {AplicacionesWeb, Algoritmos, Modelacion, Bioprocesos, Hidraulica, Señales  }
    public enum FormatoImpresion { Negativo,Positivo,Normal}
    public enum BloquesHorarios { i8_30a9_20, i9_30a10_20, i10_30a11_20, i11_30a12_20, i12_30a13_20, i13_30a14_20, i14_30a15_20, i15_30a16_20, i16_30a17_20, i17_30a18_20, i18_30a19_20, i19_30a20_20, i20_30a21_20, i21_30a22_20 }
    static class Aplicacion
    {
        static List<Usuario> usuarios = new List<Usuario>();
        public static List<CursoCurricular> cursos = new List<CursoCurricular>();
        public static Usuario usuarioActual;

        public static List<CursoCurricular> GetCursoCurricular() { return cursos; }
        public static List<Usuario> GetUsuarios() { return usuarios; }

        public static void IniciarSesion()
        {
            
            InicioSesion:
            Console.Clear();
            Console.Write("Ingrese su nombre: ");
            string nombreUsuario = Console.ReadLine();
            Console.Write("Ingrese su contraseña: ");
            string contraseña = Console.ReadLine();

            foreach (Usuario usuario in usuarios)
            {
                if (usuario is Estudiante)
                {
                    Estudiante estudiante = (Estudiante)usuario;
                    if (usuario.nombre == nombreUsuario && usuario.contraseña == contraseña)
                    {
                        usuarioActual = usuario;
                        Console.Clear();
                        PlataformaEstudiante.MenuPlataformaEstudiante(estudiante); return;
                    }
                }
                if (usuario is Administrador)
                {
                    Administrador administrador = (Administrador)usuario;
                    if (usuario.nombre == nombreUsuario && usuario.contraseña == contraseña)
                    {
                        usuarioActual = usuario;
                        Console.Clear();
                        Gestor.MenuGestor(administrador); return;
                    }
                }
            }

            Program.ImprimirNegativo("Usuario o contraseña invalidos\n");
            Console.WriteLine("Que desea hacer: \n" +
                            "1. Volver a iniciar sesion\n" +                                                                               
                            "2. Registrarse\n" +                                                                   
                            "3. Salir al menu principal");
            int opcion = Program.ChequearOpcion(1, 3);
            if (opcion == 1) goto InicioSesion;
            if (opcion == 2) Registrarse();
            if (opcion == 3) return;
            //
            return;
        }

        public static void Registrarse()
        {
            
            Console.Write("Ingrese su nombre: ");
            string nombreUsuario = Console.ReadLine();

            if (NombresUsuarios().Contains(nombreUsuario))
            {
                do
                {
                    Console.WriteLine("Ese nombre de usuario ya existe, ingrese otro: ");
                    nombreUsuario = Console.ReadLine();

                } while (NombresUsuarios().Contains(nombreUsuario));
            }
            

            Console.Write("Ingrese su contraseña: ");
            string contraseña = Console.ReadLine();
            Console.WriteLine("Ingrese su especialidad: ");
            for (int i = 0; i <= 5; i++)
            {
                Console.WriteLine(i+1 + ". " + Enum.GetName(typeof(Especialidad), i));
            }
            Especialidad especialidad = (Especialidad)Program.ChequearOpcion(1, 7);
            List <CursoCurricular>avanceMalla = new List<CursoCurricular>();
            #region
            /*
            Console.WriteLine("Ingrese los cursos de su avance de malla");
            List<CursoCurricular> cursosCurriculares = GetCursosCurriculares();
            
            bool flag = true;
            int cont = 0;
            do
            {
                for (int i = 1; i < cursosCurriculares.Count(); i++)
                {

                    if (cursos[i].tipo.Equals(TipoCurso.Curricular))
                    {
                        Console.WriteLine(i + ". " + cursosCurriculares[i-1].nombre);
                        cont++;
                    }
                }
                Console.WriteLine("Ingrese su curso:");
                int opcion2 = Program.ChequearOpcion(1, cont);
                avanceMalla.Add(cursosCurriculares[opcion2-1]);
                Program.ImprimirPositivo("Curso agregado a su avance de malla");
                Console.WriteLine("¿Desea agregar otro curso?\n1. Si\n2. No");
                opcion2 = Program.ChequearOpcion(1, 2);
                if (opcion2 == 2) flag = false;
            } while (flag); */
            #endregion
            Console.WriteLine("Ingrese su concentracion:");
            for (int i= 0; i < 6; i++)
            {
                Console.WriteLine(i + 1 + ". " + Enum.GetName(typeof(Concentracion), i));
            }

            Concentracion concentracion = (Concentracion)Program.ChequearOpcion(1, 6);
            usuarios.Add(new Estudiante(avanceMalla, especialidad, concentracion, nombreUsuario, contraseña, false));
            Console.Clear();
            Program.ImprimirPositivo("Usuario Creado");
            return;
        }

        public static List<string> NombresUsuarios()
        {
            List<string> retorno = new List<string>();
            foreach (Usuario u in usuarios)
            {
                retorno.Add(u.nombre);
            }
            return retorno;
        }

        public static List<CursoCurricular> GetCursosCurriculares()
        {
            List<CursoCurricular> retorno = new List<CursoCurricular>();
            foreach(CursoCurricular c in cursos)
            {
                retorno.Add(c);
            }
            return retorno;
        }


        public static void CargarCursos(string fileName = "dataCursosDisponibles.csv")
        {
            try
            {

                string path = Path.GetFullPath(@"..\..");

                //Console.WriteLine("Dir: "+path);
                Program.ImprimirPositivo("Dir: " + path);
                path = Path.Combine(path, @"dataCursosDisponibles.csv");


                StreamReader file = new StreamReader(path);
                string line;
                string previoNRC = string.Empty;
                List<Evento> listaEventos = new List<Evento>();
                string nombre, profesor, nrc, carrera;
                int creditos;

                List<List<string>> conjuntoCurso = new List<List<string>>();
                conjuntoCurso.Add(new List<string>() { file.ReadLine() });
                while ((line = file.ReadLine()) != null)
                {
                    string[] LA = line.Split(';');
                    nrc = LA[0];
                    if (previoNRC == nrc)
                    {
                        conjuntoCurso[conjuntoCurso.Count()-1].Add(line);
                    }
                    else
                    {
                        conjuntoCurso.Add(new List<string>());
                    }

                    previoNRC = nrc;
                }



                foreach(List<string> curso in conjuntoCurso)
                {
                    List<Evento> listaEvento = new List<Evento>();
                    foreach(string linea in curso)
                    {
                        string[] dataLinea = linea.Split(';');
                        List<string> listaHorario = new List<string>();
                        for(int i = 6; i < 12; i++)
                        {
                            if(dataLinea[i] != "")
                            {
                                string dia = dataLinea[12].Replace('-',':');
                                if(dataLinea[12] == "") { dia = "A"; }
                                listaHorario.Add((dataLinea[i]+":"+dia).Replace(" -",":"));
                            }
                        }
                        for (int i = 0; i < listaHorario.Count; i++)
                        {
                            //Console.WriteLine(listaHorario[i]);
                            string horario = listaHorario[i];
                            var papa = (horario.ToString().Split(':'));

                            //Console.WriteLine(papa[0]);

                        }

                        if (linea == curso[curso.Count()-1])
                        {
                            string[] linea2 = linea.Split(';');
                            nombre = linea2[4];
                            profesor = linea2[15];
                            nrc = linea2[0];
                            carrera = linea2[1];
                            creditos = Convert.ToInt32(linea2[5]);
                            CursoCurricular cursoCurricular = new CursoCurricular(nrc, creditos, new List<CursoCurricular>(),
                                Especialidad.ICA, listaEventos, nombre, profesor,listaEventos, TipoCurso.Curricular);
                            cursos.Add(cursoCurricular);
                        }



                    }

                }

                List<string> formatoHorario(string stringHorario)
                {

                    List<string> returnString = new List<string>();
                    for(int i = 0; i < stringHorario.Split(',').Count(); i++)
                    {
                        returnString.Add(stringHorario.Split(',')[i]);
                        //Console.WriteLine(stringHorario.Split(',')[i]);
                    }
                    //Console.WriteLine();
                    return returnString;
                }
                


                file.Close();
            }
            catch (FileNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
            }


        }

        public static void CargarUsuarios(string fileName = "dataUsuarios.csv")
        {
            try
            {

                string path = Path.GetFullPath(@"..\..");

                //Console.WriteLine("Dir: "+path);
                Program.ImprimirPositivo("Dir: " + path);
                path = Path.Combine(path, @"saveData.csv");


                StreamReader file = new StreamReader(path);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] LI = line.Split(';');
                    string nombre = LI[0];
                    string contraseña = LI[1];
                    bool admin;
                    //
                    if (LI[2] == "true")
                    {
                        admin = true;
                        Administrador administrador = new Administrador(nombre, contraseña, admin);
                        usuarios.Add(administrador);
                    }
                    else
                    {
                        string especialidad, añoIngreso, concentracion, avanceMalla;
                        especialidad = LI[4];
                        añoIngreso = LI[5];
                        concentracion = LI[6];
                        avanceMalla = LI[7];
                        List<CursoCurricular> listaAvanceMalla = new List<CursoCurricular>();

                        foreach (string nrc in LI[7].Split(','))
                        {
                            CursoCurricular curso = cursos.Find(x => x.nrc == nrc);
                            listaAvanceMalla.Add(curso);
                        }
                        Estudiante estudiante = new Estudiante(listaAvanceMalla, Especialidad.ICC, Concentracion.Hidraulica, nombre, contraseña, false);
                        usuarios.Add(estudiante);
                    }
                }

                file.Close();
            }
            catch (FileNotFoundException e1)
            {
                System.Console.WriteLine(e1.Message);
            }


        }

        public static void MostrarUsuarios()
        {
            Console.Clear();
            Program.ImprimirNegativo("--Imprimir Usuarios--\n");
            foreach(Usuario usuario in usuarios)
            {
                Console.WriteLine($"Nombre: {usuario.nombre}, Admin: {usuario.esAdmin}");
            }
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static void print()
        {
            foreach(Usuario usuario in usuarios)
            {
                Console.WriteLine(usuario.nombre+" "+usuario.contraseña);
            }
        }

        public static void GuardarData()
        {

            //string path = Directory.GetCurrentDirectory();

            string path = Path.GetFullPath(@"..\..");

            //Console.WriteLine("Dir: "+path);
            Program.ImprimirPositivo("Dir: " + path);
            path = Path.Combine(path, @"saveData.csv");


            StreamWriter saveFile = new StreamWriter(path);
            foreach(Usuario usuario in usuarios)
            {
                saveFile.Write($"{usuario.nombre};{usuario.contraseña};" +
                    $"{usuario.esAdmin};");

                if (usuario is Estudiante)
                {
                    Estudiante estudiante = (Estudiante)usuario;
                    saveFile.Write($"ING;{estudiante.especialidad};0000;{estudiante.concentracion};1,2,3,4,5,6\n");
                }
                else
                {
                    saveFile.Write("\n");
                }


            }
            saveFile.Close();
            Program.ImprimirPositivo("Guardado Exitoso.\n");
            
            return;
        }



    }
}
