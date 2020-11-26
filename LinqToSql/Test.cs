using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSql
{
    
    public class Test
    {
        const string conn = @"Persist Security Info = False; Integrated Security = True; Initial Catalog = CinemaDb; Server = WINAPIUZIYVIF6L\SQLEXPRESS";

        public static void SelectMovies()
        {
            // istanziamo il DataContext, che va le veci del db
            DataClasses1DataContext db = new DataClasses1DataContext(conn);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2} - {3}",movie.ID, movie.Titolo, movie.Genere, movie.Durata);
            }

            Console.ReadKey();
        }

        public static void FilterMovieByGenere()
        {
            DataClasses1DataContext db = new DataClasses1DataContext(conn);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2} - {3}", movie.ID, movie.Titolo, movie.Genere, movie.Durata);
            }

            Console.WriteLine("\n \n Inserisci il genere:");
            string genere;
            genere = Console.ReadLine();

            IQueryable<Movy> moviesByGenere = // avrei potuto scrivere var 
                from m in db.Movies
                where m.Genere == genere
                select m;

            foreach (var film in moviesByGenere)
            {
                Console.WriteLine($"\n {film.ID}{film.Titolo}{film.Durata}");     
            }

            Console.ReadKey();

        }

        public static void InsertMovie()
        {
            DataClasses1DataContext db = new DataClasses1DataContext(conn);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2} - {3}", movie.ID, movie.Titolo, movie.Genere, movie.Durata);
            }

            var new_movie = new Movy();
            new_movie.Titolo = "Jumanji";
            new_movie.Genere = "Fantasy";
            new_movie.Durata = 145;

            db.Movies.InsertOnSubmit(new_movie);
            // l'inserimento avviene quando faccio SubmitChanges


            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            SelectMovies();
        }

        public static void DeleteMovie()
        {
            DataClasses1DataContext db = new DataClasses1DataContext(conn);

            var delete_movie = db.Movies.SingleOrDefault(m => m.ID == 2);
            
            if (delete_movie != null)
            {
                db.Movies.DeleteOnSubmit(delete_movie);
            }


            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            SelectMovies();
        }

        public static void UpdateMovieByTitolo()
        {
            DataClasses1DataContext db = new DataClasses1DataContext(conn);

            Console.WriteLine("Inserisci il titolo del film da aggiornare:");
            string titolo = Console.ReadLine();

            var filmByTitolo =
                from film in db.Movies
                where film.Titolo == titolo
                select film;

            Console.WriteLine($"Ho trovato {filmByTitolo.Count()} film con titolo {titolo}:");
            
            if (filmByTitolo.Count() == 0) // se non c'è un film con quel titolo non voglio aggiornare uno
            {
                return; // esce anticipatamente dalla funzione
            }

            if (filmByTitolo.Count() > 1) // altrimenti li aggiorno tutti
            {
                return;
            }

            Console.WriteLine("\n Aggiornare il titolo:");
            string new_titolo = Console.ReadLine();

            Console.WriteLine("Aggiornare il genere:");
            string new_genere = Console.ReadLine();

            Console.WriteLine("Aggiornare la durata:");
            int new_durata = Convert.ToInt32(Console.ReadLine());

            foreach (var f in filmByTitolo) // è qui che aggiorno i dati
            {
                f.Titolo = new_titolo;
                f.Genere = new_genere;
                f.Durata = new_durata;
            }

            try
            {
                Console.WriteLine("Premi un tasto per mandare le modifiche al db");
                Console.ReadKey();
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                // FailOnFirstConflict = manda l'eccezione subito la prima volta in cui qualcosa fallisce
                // La seconda opzione avrebbe accumulato tutte le eccezioni eventuali e le avrebbe mandate tutti insieme alla fine 
            }
            catch (ChangeConflictException e) // eccezione che gestisce i conflitti per concorrenza
            {
                Console.WriteLine("\n Concurrency error");
                Console.WriteLine(e);
                Console.ReadKey();

                db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues);
                
                db.SubmitChanges();
            }
        }

    }
}
