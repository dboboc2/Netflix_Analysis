//
// BusinessTier:  business logic, acting as interface between UI and data store.
//
// Netflix  Database  Application  using  N-Tier  Design.
//
// << Daniel Boboc >>
// U. of Illinois, Chicago
// CS341, Spring 2018
// Final Project
// 

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    //
    // Constructor:
    //
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    //
    // TestConnection:
    //
    // Returns true if we can establish a connection to the database, false if not.
    //
    public bool TestConnection()
    {
      return dataTier.TestConnection();
    }


    //
    // GetNamedUser:
    //
    // Retrieves User object based on USER NAME; returns null if user is not
    // found.
    //
    // NOTE: there are "named" users from the Users table, and anonymous users
    // that only exist in the Reviews table.  This function only looks up "named"
    // users from the Users table.
    //
    public User GetNamedUser(string UserName)
    {
            // get the userID and occupation
            string IDquery, OCCquery;
            // build the query
            UserName = UserName.Replace("'", @"''"); // BIG LINE RIGHT HERE, if a movie has a ' in it, then it gets replaced by a double ' ('').
            IDquery = String.Format(@"SELECT UserID FROM Users WHERE UserName = '{0}' ", UserName);
            OCCquery = String.Format(@"SELECT Occupation FROM Users WHERE UserName = '{0}' ", UserName);
            
            // receiving 1 object, so Scalar Query
            object ID = dataTier.ExecuteScalarQuery(IDquery);

            // receiving 1 object, so Scalar Query
            object OCC = dataTier.ExecuteScalarQuery(OCCquery);

            // check if they wasnt found.
            if (ID == null)
                return null;
            
            string occupation;
            if (OCC == null || OCC.ToString() == "")
                occupation = "Unemployed";
            else
                occupation = OCC.ToString(); // also grab their occupation HAD TO DO .toString() rather than (string) cast WEIRD

            int id = (int) ID; // grab the id from the object
            
            User user = new User(id , UserName, occupation); // create new object then return it

            return user;
    }


    //
    // GetAllNamedUsers:
    //
    // Returns a list of all the users in the Users table ("named" users), sorted 
    // by user name.
    //
    // NOTE: the database also contains lots of "anonymous" users, which this 
    // function does not return.
    //
    public IReadOnlyList<User> GetAllNamedUsers()
    {
            List<User> users = new List<User>();
            string query;
            // build the query
            query = @"SELECT * FROM Users ORDER BY UserName";
            // receiving 1 object, so Scalar Query
            DataSet USERS = dataTier.ExecuteNonScalarQuery(query);

            // check if it wasnt found.
            if (USERS == null)
                return null;

            string occupation;
            // traverse the dataset and add each User's data to a List format of User objects
            foreach (DataTable table in USERS.Tables){
                foreach (DataRow row in table.Rows){
                    if (row != null){
                        // check to see if Occupation is empty
                        if (row["Occupation"] == null)
                            occupation = ""; // if it's empty make it an empty string;
                        else
                            occupation = row["Occupation"].ToString(); // otherwise, store the occupation string in this string variable, had to do .tostring() rather than (string) cast
                        
                        User rev = new User((int)row["UserID"], row["UserName"].ToString(), occupation); // create the user and add it to the list.
                        users.Add(rev); // append object to list
                    }
                }
            }

            return users; // finally, return the list
    }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE ID; returns null if movie is not
    // found.
    //
    public Movie GetMovie(int MovieID) {
            string query;
            // build the query
            query = String.Format(@"SELECT MovieName FROM Movies WHERE MovieID = {0}", MovieID);
            // receiving 1 object, so Scalar Query
            object MOVIE = dataTier.ExecuteScalarQuery(query);

            // check if it wasnt found.
            if (MOVIE == null)
                return null;

            // cast the object into an int
            string MovieName = (string) MOVIE;
            
            // create the movie object to return
            Movie m = new Movie(MovieID, MovieName);
            return m;
        }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE NAME; returns null if movie is not
    // found.
    //
    public Movie GetMovie(string MovieName)
    {
            string query;
            // build the query

            MovieName = MovieName.Replace("'", @"''"); // BIG LINE RIGHT HERE, if a movie has a ' in it, then it gets replaced by a double ' ('').
            query = String.Format(@"SELECT MovieID FROM Movies WHERE MovieName = '{0}'", MovieName);
            
            // receiving 1 object, so Scalar Query
            object MOVIE = dataTier.ExecuteScalarQuery(query);

            // check if it wasnt found.
            if (MOVIE == null)
                return null;

            // cast the object into an int
            int MovieID = (int) MOVIE;

            // create the movie object to return
            Movie m = new Movie(MovieID, MovieName);
            return m;
    }


    //
    // AddReview:
    //
    // Adds review based on MOVIE ID, returning a Review object containing
    // the review, review's id, etc.  If the add failed, null is returned.
    //
    public Review AddReview(int MovieID, int UserID, int Rating)
    {
            Movie MOVIE = GetMovie(MovieID);    // call the function above this one to get the movie object (to get the string)
            string moviename = MOVIE.MovieName; // grab the moviename string from the object
            moviename = moviename.Replace("'", @"''"); // BIG LINE RIGHT HERE, if a movie has a ' in it, then it gets replaced by a double ' ('').

            // this query does two commands in one sql string
            // it inserts the review and grabs the new unique review ID
            // the VALUES() can have all the columns in order that you want them inputted in, without having to specify which column.
            // doesn't need to be Reviews(col1,col2) BC we are inserting into every column
            // NVM I AM INSERTING THE LAST 3 COLUMNS AND LETTING THE SERVER MAKE THE FIRST COLUMN FOR ME AND GRABBING THAT FIRST COLUMN BY USING SCOPE_IDENTITY()
            string query = string.Format(@"INSERT INTO Reviews(MovieID, UserID, Rating) VALUES({0}, {1}, {2});
                                           SELECT ReviewID FROM Reviews WHERE ReviewID = SCOPE_IDENTITY();
                                           ", MovieID, UserID, Rating);

            
            object newReviewID = dataTier.ExecuteScalarQuery(query); // This atm is basically an int object holding the primary key of the review added in

            //check if it properly inserted
            if (newReviewID == null)
                return null;

            int ReviewID = (int)newReviewID; // grab the returned int/id

            Review REVIEW = new Review(ReviewID, MovieID, UserID, Rating);
            return REVIEW;
    }


    //
    // GetMovieDetail:
    //
    // Given a MOVIE ID, returns detailed information about this movie --- all
    // the reviews, the total number of reviews, average rating, etc.  If the 
    // movie cannot be found, null is returned.
    //
    public MovieDetail GetMovieDetail(int MovieID)
    {
            string query;
            
            //////////////// get the name
            query = String.Format(@"SELECT MovieName FROM Movies WHERE MovieID = {0}", MovieID);
            // receiving 1 object, so Scalar Query
            object MOVIENAME = dataTier.ExecuteScalarQuery(query);

            if (MOVIENAME == null)
                return null;
            /////////////// end



            /////////////// get the AvgRating and NumReviews
            double scoreTotal, NumReviews, AVG;

            // used coalesce so that it would return 0 for SUM if no ratings are found, rather than a weird null.
            query = String.Format(@"SELECT COALESCE(SUM(Rating), 0) AS Sum, COALESCE(COUNT(Rating), 0) AS Count FROM Reviews WHERE MovieID = {0}", MovieID);

            // receiving 2 things, so NonScalar and returns a dataset rather than an object 
            DataSet RATINGDATA = dataTier.ExecuteNonScalarQuery(query);
            if (RATINGDATA == null)
                return null;


            DataRow X = RATINGDATA.Tables[0].Rows[0];
            
            scoreTotal = (int) (X["Sum"]);
            NumReviews = (int) X["Count"];

            if (scoreTotal == 0) // so it doesnt return NaN instead, it will return 0 if no reviews are found
                AVG = 0;
            else
                AVG = (double)scoreTotal / (double)NumReviews;
            ///////////////



            /////////////// get the list of reviews
            List<Review> ReviewList = new List<Review>(); // the List of Reviews to return

            query = String.Format(@"SELECT * FROM Reviews WHERE MovieID = {0} ORDER BY Rating DESC", MovieID);

            // receiving 2 things, so NonScalar and returns a dataset rather than an object 
            DataSet REVIEWS = dataTier.ExecuteNonScalarQuery(query);
            if (REVIEWS == null)
                return null;

            // traverse the dataset and add each review's data to a List format of Review objects
            foreach (DataTable table in REVIEWS.Tables){
                foreach (DataRow row in table.Rows)
                {
                    //foreach(DataRow row in REVIEWS.Tables[0].Rows) {
                    if (row != null)
                    {
                        Review rev = new Review((int)row["ReviewID"], (int)row["MovieID"], (int)row["UserID"], (short)row["Rating"]);
                        ReviewList.Add(rev);
                    }
                }
            }
            /////////////// end



            /////////////// Put it all together and return it
            Movie m = new Movie(MovieID, (string) MOVIENAME); // create the movie object
            double AVG2 = Math.Round(AVG, 5); // round the avg for the avgreview double
            // DECIDED TO TRUNCATE IT LESS ^^^^ BECAUSE IT MESSES UP THE WAY HE WANTS IT SORTED 
            MovieDetail DETAIL = new MovieDetail(m, AVG2, (int) NumReviews, ReviewList); // create the detail object.

            return DETAIL;
            /////////////// end

        }


    // when called this will return a list of movie objects including movieNames and occupations
    public IReadOnlyList<Movie> GetAllMovies()
    {
            List<Movie> movies = new List<Movie>();
            string query;
            // build the query
            query = @"SELECT * FROM Movies ORDER BY MovieName";

            DataSet MOVIES = dataTier.ExecuteNonScalarQuery(query);

            // check if it wasnt found.
            if (MOVIES == null)
                return null;
            
            // traverse the dataset and add each Movie's data to a List format of Movie objects
            foreach (DataTable table in MOVIES.Tables){
                foreach (DataRow row in table.Rows){
                    if (row != null){
                        Movie mov = new Movie((int)row["MovieID"], row["MovieName"].ToString()); // create the movie object
                        movies.Add(mov); // append object to list
                    }
                }
            }

            return movies; // finally, return the list
        }


        
        //
        // GetUserDetail:
        //
        // Given a USER ID, returns detailed information about this user --- all
        // the reviews submitted by this user, the total number of reviews, average 
        // rating given, etc.  If the user cannot be found, null is returned.
        //
        public UserDetail GetUserDetail(int UserID)
        {
            string query;

            //////////////// (this gets the User object) get the UserName and then use GetNamedUser(string username) to get the full object also containing their occupation
            query = String.Format(@"SELECT UserName FROM Users WHERE UserID = {0}", UserID);
            // receiving 1 object, so Scalar Query
            object USERNAME = dataTier.ExecuteScalarQuery(query);

            if (USERNAME == null) // was it a valid ID?
                return null;

            string userName = USERNAME.ToString(); // convert the name to string

            User USER = GetNamedUser(userName); // get the USER object (which gets stored in UserDetail later)
            /////////////// end



            /////////////// get the AvgRating and NumReviews (gets stored in UserDetail object later on)
            double scoreTotal, NumReviews, AVG;

            // used coalesce so that it would return 0 for SUM if no ratings are found, rather than a weird null.
            query = String.Format(@"SELECT COALESCE(SUM(Rating), 0) AS Sum, COALESCE(COUNT(Rating), 0) AS Count FROM Reviews WHERE UserID = {0}", UserID);

            // receiving 2 things, so NonScalar and returns a dataset rather than an object 
            DataSet RATINGDATA = dataTier.ExecuteNonScalarQuery(query);
            if (RATINGDATA == null)
                return null;
            
            DataRow X = RATINGDATA.Tables[0].Rows[0]; // grab the first row from the returned data set table
            scoreTotal = (int)(X["Sum"]); // grab the sum value
            NumReviews = (int)X["Count"]; // grab the count value

            if (scoreTotal == 0) // checks to see if no reviews are found, returning 0 if none are found.
                AVG = 0;
            else
                AVG = (double)scoreTotal / (double)NumReviews; // divide them to compute the avg
            ///////////////



            /////////////// get the list of reviews
            List<Review> ReviewList = new List<Review>(); // the List of Reviews to return

            query = String.Format(@"SELECT * FROM Reviews WHERE UserID = {0} ORDER BY Rating DESC", UserID);

            // receiving 2 things, so NonScalar and returns a dataset rather than an object 
            DataSet REVIEWS = dataTier.ExecuteNonScalarQuery(query);
            if (REVIEWS == null)
                return null;

            // traverse the dataset and add each review's data to a List format of Review objects
            foreach (DataTable table in REVIEWS.Tables){
                foreach (DataRow row in table.Rows){
                    if (row != null){
                        Review rev = new Review((int)row["ReviewID"], (int)row["MovieID"], (int)row["UserID"], (short)row["Rating"]);
                        ReviewList.Add(rev);
                    }
                }
            }
            /////////////// end



            /////////////// Put it all together and return it
            double AVG2 = Math.Round(AVG, 4); // round the avg for the avgreview double, truncating it
            UserDetail DETAIL = new UserDetail(USER, AVG2, (int)NumReviews, ReviewList); // create the detail object.

            return DETAIL;
            /////////////// end
        }


        //
        // GetTopMoviesByAvgRating:
        //
        // Returns the top N movies in descending order by average rating.  If two
        // movies have the same rating, the movies are presented in ascending order
        // by name.  If N < 1, an EMPTY LIST is returned.
        //
        // ORDER BY category_id DESC, product_name ASC LIMIT N;
        public IReadOnlyList<Movie> GetTopMoviesByAvgRating(int N){
            List<Movie> movies = new List<Movie>();
            if (N < 1) // if less than minimum amount return the empty list
                return movies;
            
            ///////////////
            List<Movie> movies2 = new List<Movie>(); // This will be used to hold the object lists where i'll grab ID's from

            movies2 = (List<Movie>) GetAllMovies(); // casted it so it's not just a read only :)
            string query;
            // TOP allows me to only grab the top N entries out of the table
            query =  String.Format(@"select TOP {0} Movies.MovieName, avg(Cast(Reviews.Rating as Float)) as AR
                                     FROM Reviews
                                     INNER JOIN Movies on Reviews.MovieID = Movies.MovieID
                                     group by Movies.MovieName
                                     order by AR DESC, Moviename ASC
                                     ", N);

            DataSet TOPN = dataTier.ExecuteNonScalarQuery(query);

            string MOVIENAME;
            int MOVIEID;
            foreach (DataTable table in TOPN.Tables){
                foreach (DataRow row in table.Rows){
                    if (row != null){
                        MOVIENAME = row["MovieName"].ToString();
                        MOVIEID = GetMovie(MOVIENAME).MovieID;
                        Movie rev = new Movie(MOVIEID, MOVIENAME);
                        movies.Add(rev);
                    }
                }
            }
           
            return movies;

            /* // OLD ATTEMPT (TOO SLOW, small took 9 seconds, big crashed)
            List<MovieDetail> movieDetailList = new List<MovieDetail>(); // had to initialize it :/

            // iterate through the movienames, get the detail object for each movie, append it to a new list
            foreach (Movie X in movies2) {
                MovieDetail tmp = GetMovieDetail(X.MovieID);
                if (tmp != null)
                    movieDetailList.Add(tmp);
                else
                    continue;
            }

            double max = 0;
            int found = 0;
            MovieDetail TMP = null;
            // now Im going to grab the max amounts from the List N times.
            // for each of the movie detail objects, find the new max avg score in the entire list
            // then, if one is found, remove it from the list and append its movie object to the returned movie list
            while (N > 0){
                // put a parallel foreach loop to hopefully speed up this non race conditioned loop. Doesn't seem to be a difference though
                // As Visual Studio seems to do it automatically anyways :/... The NON-parallel foreach loop is commented out right below the while loop
                foreach (MovieDetail X in movieDetailList){
                    if (X.AvgRating > max){
                        max = X.AvgRating;
                        TMP = X;
                        found = 1;
                    }
                }

                if (found == 1){
                    movies.Add(TMP.movie);
                    movieDetailList.Remove(TMP);
                    N--;
                }
                else
                    break;

                // reset variables
                found = 0;
                max = 0;
            }
            return movies; // return the list
            */






            /*
            foreach(MovieDetail X in movieDetailList){
                    if(X.AvgRating > max){
                        max = X.AvgRating;
                        TMP = X;
                        found = 1;
                    }
            }
            */
            //////////////////////// end


            /* // OLD SLOWER VERSION IGNORE THIS (still works tho :'{ )
            MovieDetail X, Y;
            // sort lamba function
            // ORIGINALLY I had sorted it then grabbed the first N, instead, I will just find the Max then remove it and add it to the list
            movies.Sort((x, y) => {
                X = GetMovieDetail(x.MovieID);
                Y = GetMovieDetail(y.MovieID);
                var ret = X.AvgRating.CompareTo(Y.AvgRating);
                if (ret == 0) ret = x.MovieName.CompareTo(y.MovieName);
                return ret;
            });

            List<Movie> movies2 = null;

            Movie [] movieArray = movies.ToArray();

            for (int i = 0; i < N; i++)
                movies2.Add(movieArray[i]);
                
            */
            /////////////// end

        }


    }//class
}//namespace
