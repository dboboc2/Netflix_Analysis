//
// Netflix  Database  Application  using  N-Tier  Design.
//
// << Daniel Boboc >>
// U. of Illinois, Chicago
// CS341, Spring 2018
// Project 08
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace proj8_dboboc2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.textBox1.Text = "netflix-200k.mdf"; // SO THAT THERE IS A DEFAULT DATABASE, USER DOESNT HAVE TO ENTER IT EVERY TIME
        }


        private bool fileExists(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                string msg = string.Format("Input Database file not found: '{0}'",
                  filename);

                MessageBox.Show(msg);
                return false;
            }

            // exists!
            return true;
        }


        //#2 to get all movies alphabetized into the list
        private void button1_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text; // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;
            

            BusinessTier.Business biztier = new BusinessTier.Business(filename);

            // grabs a list of all the movies (ID and MovieName inside each object)
            IReadOnlyList<BusinessTier.Movie> MOVIES = biztier.GetAllMovies();
            
            //iterate through the lsit and add them to the listbox
            foreach(BusinessTier.Movie movie in MOVIES)
                this.listBox1.Items.Add(movie.MovieName);
        }

        //#2
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // retrieve the NAME on the MOVIE based on the CLICKED entry in the list
            var MOVIE = biztier.GetMovie(this.listBox1.Text);
            
            // CLEAR THE LISTBOXES FOR THE NEW ENTRY
            this.listBox3.Items.Clear();
            this.listBox4.Items.Clear();

            
            int movieID;
            if (MOVIE != null)
                movieID = MOVIE.MovieID;
            else{
                this.listBox3.Items.Add("Movie not Found");
                this.listBox4.Items.Add("Movie not Found");
                return;
            }

            // add the movie's ID
            this.listBox3.Items.Add(movieID.ToString());

            // use movie id to get movie class
            var movieINFO = biztier.GetMovieDetail(movieID);
            // grab the average rating from it
            double avgRating = movieINFO.AvgRating;
            // input it into the listbox
            if(avgRating == 0)
                this.listBox4.Items.Add("No Reviews");
            else
                this.listBox4.Items.Add(avgRating.ToString());
        }

        // #3 button click
        private void button2_Click(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRIES
            this.listBox2.Items.Clear();

            string filename = this.textBox1.Text; // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;

            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            IReadOnlyList<BusinessTier.User> user = biztier.GetAllNamedUsers();  // now we can call functions on this object which serves as a membrane for the DB

            foreach (var i in user)                 // var is like auto (type inference) 
                this.listBox2.Items.Add(i.UserName);
        }



        // when they click on someone for #3, put their ID and OCCUPATION up.
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // retrieve the info on the user based on their username
            var USER = biztier.GetNamedUser(this.listBox2.Text);

            // CLEAR THE LISTBOXES FOR THE NEW ENTRY
            this.listBox5.Items.Clear();
            this.listBox6.Items.Clear();

            // now add their userID
            this.listBox5.Items.Add(USER.UserID);

            // now add their occupation
            if (USER.Occupation == "")
                this.listBox6.Items.Add("unemployed");
            else
                this.listBox6.Items.Add(USER.Occupation);
     
        }
        // #4 activate
        private void button3_Click(object sender, EventArgs e)
        {
            // CLEAR THE LIST
            this.listBox9.Items.Clear();
            this.listBox7.Items.Clear();

            ////////////// PUTTING THE MOVIES IN THE LIST BOX

            string filename = this.textBox1.Text; // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
                return;

            BusinessTier.Business biztier = new BusinessTier.Business(filename);

            // grabs a list of all the movies (ID and MovieName inside each object)
            IReadOnlyList<BusinessTier.Movie> MOVIES = biztier.GetAllMovies();

            //iterate through the lsit and add them to the listbox
            foreach (BusinessTier.Movie movie in MOVIES)
                this.listBox9.Items.Add(movie.MovieName);
            ////////////// END PUTTING THE MOVIES IN THE LIST BOX

        }

        private void listBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox7.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // retrieve the info on the Movie based on their SELECTED CHOICE
            var MOVIE = biztier.GetMovie(this.listBox9.Text); // get the movie object (to get id num)
            int MOVIEID = MOVIE.MovieID; // get the id num
            var movieDetail = biztier.GetMovieDetail(MOVIEID); // get the details (to get reviews)
            IReadOnlyList<BusinessTier.Review> reviews = movieDetail.Reviews; // put the reviews into a list


            // check if 0 reviews, if not, put all the reviews into tht listbox.
            if (movieDetail.NumReviews == 0)
                this.listBox7.Items.Add("No reviews for this movie");
            else {
                // print name
                this.listBox7.Items.Add(MOVIE.MovieName);
                // print a space
                this.listBox7.Items.Add("");

                // print all reviews
                foreach (var i in reviews)
                    this.listBox7.Items.Add(String.Format("{0}: {1}", i.UserID, i.Rating));

            }

        }
        
        // when they type one for #4
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox7.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // retrieve the info on the Movie based on their SELECTED CHOICE
            var MOVIE = biztier.GetMovie(this.textBox2.Text); // get the movie object (to get id num)
            if (MOVIE == null){  // if the entered moviename isnt found
                this.listBox7.Items.Add("Movie entered was not found");
                return;
            }
            int MOVIEID = MOVIE.MovieID; // get the id num
            var movieDetail = biztier.GetMovieDetail(MOVIEID); // get the details (to get reviews)
            IReadOnlyList<BusinessTier.Review> reviews = movieDetail.Reviews; // put the reviews into a list


            // check if 0 reviews, if not, put all the reviews into tht listbox.
            if (movieDetail.NumReviews == 0)
                this.listBox7.Items.Add("No reviews for this movie");
            else
            {
                // print name
                this.listBox7.Items.Add(MOVIE.MovieName);
                // print a space
                this.listBox7.Items.Add("");

                // print all reviews
                foreach (var i in reviews)
                    this.listBox7.Items.Add(String.Format("{0}: {1}", i.UserID, i.Rating));

            }
        }

        // when they click the button for #5
        private void button4_Click(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOXES FOR THE NEW ENTRIES
            this.listBox10.Items.Clear();
            this.listBox8.Items.Clear();
            this.textBox3.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            if (!fileExists(filename)) // checks if the database file is valid
                return;
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database
            // grab the whole list of USERS
            IReadOnlyList<BusinessTier.User> USERS = biztier.GetAllNamedUsers();

            // for each user in the list put their name into the list box
            foreach (var i in USERS)
                this.listBox10.Items.Add(i.UserName);

        }

        // when they select something in the list for #5
        private void listBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox8.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // grab the object on the selected person's username (to get their id)
            var USER = biztier.GetNamedUser(this.listBox10.Text);
            // now grab their userid
            int userID = USER.UserID;
            // now use their ID to get their review info
            var userDETAIL = biztier.GetUserDetail(userID);
            // now finally, put their review information in a LIST
            IReadOnlyList<BusinessTier.Review> REVIEWS = userDETAIL.Reviews;

            // print the user's username
            this.listBox8.Items.Add(USER.UserName);
            this.listBox8.Items.Add(""); // a newline


            string movieName, review;
            int reviewScore;
            // for each element in the reviews, print it out onto the listbox.
            // check if 0 reviews, if not, put all the reviews into tht listbox.
            if (userDETAIL.NumReviews == 0){ // if no reviews
                this.listBox8.Items.Add("No reviews for this user");
            }
            else{
                foreach (var i in REVIEWS)
                {
                    movieName = (biztier.GetMovie(i.MovieID)).MovieName;
                    reviewScore = i.Rating;
                    review = String.Format("{0} -> {1}", movieName, reviewScore);
                    this.listBox8.Items.Add(review);
                }
            }
        }

        // when they type something into #5, the only thing different
        // in this compared to selecting from the list is that I add
        // what to do when the name searched ISN'T a valid username for a user.
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox8.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // grab the object on the selected person's username (to get their id)
            var USER = biztier.GetNamedUser(this.textBox3.Text);
            if (USER == null){ // if the username isn't a valid user
                this.listBox8.Items.Add("username entered");
                this.listBox8.Items.Add("isn't a valid user");
                return;
            }
            // now grab their userid
            int userID = USER.UserID;
            // now use their ID to get their review info
            var userDETAIL = biztier.GetUserDetail(userID);
            // now finally, put their review information in a LIST
            IReadOnlyList<BusinessTier.Review> REVIEWS = userDETAIL.Reviews;

            // print the user's username
            this.listBox8.Items.Add(USER.UserName);
            this.listBox8.Items.Add(""); // a newline


            string movieName, review;
            int reviewScore;
            // for each element in the reviews, print it out onto the listbox.
            foreach (var i in REVIEWS)
            {
                movieName = (biztier.GetMovie(i.MovieID)).MovieName;
                reviewScore = i.Rating;
                review = String.Format("{0} -> {1}", movieName, reviewScore);
                this.listBox8.Items.Add(review);
            }



        }

        // #6 activate button
        private void button5_Click(object sender, EventArgs e)
        {
            // CLEAR THE MOVIE LIST
            this.listBox12.Items.Clear();
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox11.Items.Clear();
            this.textBox6.Clear();

            ////////////// PUTTING THE MOVIES IN THE LIST MOVIE BOX

            string filename = this.textBox1.Text; // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
                return;
            
            // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database
            BusinessTier.Business biztier = new BusinessTier.Business(filename);
            
            // grabs a list of all the movies (ID and MovieName inside each object)
            IReadOnlyList<BusinessTier.Movie> MOVIES = biztier.GetAllMovies();

            //iterate through the lsit and add them to the listbox
            foreach (BusinessTier.Movie movie in MOVIES)
                this.listBox12.Items.Add(movie.MovieName);
            ////////////// END PUTTING THE MOVIES IN THE LIST BOX


            ////////////// BEGIN PUTTING USERS IN USER LIST BOX
            // grab the whole list of USERS
            IReadOnlyList<BusinessTier.User> USERS = biztier.GetAllNamedUsers();

            // for each user in the list put their name into the list box
            foreach (var i in USERS)
                this.listBox11.Items.Add(i.UserName);

            ////////////// END PUTTING USERS IN USER LIST BOX


        }

        private void button6_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text; // db name THEN set up membrane
            if (!fileExists(filename)) // checks if the database file is valid
                return;
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // I WILL FIRST BE GETTING THE MOVIENAME AND USERNAME SELECTED
            string movie_Name, userName;

            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox13.Items.Clear();

            // CHECK FOR MOVIE

            movie_Name = this.textBox4.Text;
            // if the user DOESN'T enter a moviename, then
            // check with one they selected
            if (movie_Name == "")
                movie_Name = this.listBox12.Text;

            // find movie object using inputted movie name
            var MOVIE = biztier.GetMovie(movie_Name);
            // if movie not found, report it then exit
            if (MOVIE == null){
                this.listBox13.Items.Add("MOVIE NOT FOUND OR NO MOVIE SELECTED");
                this.listBox11.Items.Clear();
                this.listBox12.Items.Clear();
                return;
            }

            // NOW CHECK FOR USER

            userName = this.textBox5.Text;
            // if the user DOESN'T enter a username, then
            // check with one they selected
            if (userName == "")
                userName = this.listBox11.Text;

            // find user object using inputted username
            var USER = biztier.GetNamedUser(userName);
            // if username not found, report it then exit
            if (USER == null)
            {
                this.listBox13.Items.Add("USER NOT FOUND OR NO USER SELECTED");
                this.listBox11.Items.Clear();
                this.listBox12.Items.Clear();
                return;
            }

            // NOW I NEED TO GET THE RATING INTEGER
            string rating = this.textBox6.Text;
            int RATING;
            // this switch statement will be used to convert the rating
            // to an integer while simultaneously being able to alert myself
            // later on if an improper input was given. (using the default case)
            switch (rating)
            {
                case "1":
                    RATING = 1;
                    break;
                case "2":
                    RATING = 2;
                    break;

                case "3":
                    RATING = 3;
                    break;

                case "4":
                    RATING = 4;
                    break;

                case "5":
                    RATING = 5;
                    break;

                default:
                    RATING = 0;
                    break;
            }

            // if an improper input was given, alert and exit
            if(RATING == 0)
            {
                this.listBox13.Items.Add("ENTER A RATE INTEGER 1-5");
                this.listBox11.Items.Clear();
                this.listBox12.Items.Clear();
                return;
            }

            // get the movieID and userID in order to (FINALLY) submit the new review
            int movieID = MOVIE.MovieID;
            int userID  = USER.UserID;

            var Result = biztier.AddReview(movieID, userID, RATING);
            if (Result == null){ // with the given info, the review add still failed
                this.listBox13.Items.Add("With the given info, the rating submission FAILED!");
                return;
            }
            this.listBox13.Items.Add("succesfully submitted your rating:)");
        }

        // #7 activate button
        private void button7_Click(object sender, EventArgs e)
        {
            // CLEAR THE LIST
            this.listBox15.Items.Clear();
            this.listBox14.Items.Clear();
            this.textBox7.Clear();

            ////////////// PUTTING THE MOVIES IN THE LIST MOVIE BOX

            string filename = this.textBox1.Text; // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
                return;
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database


            // grabs a list of all the movies (ID and MovieName inside each object)
            IReadOnlyList<BusinessTier.Movie> MOVIES = biztier.GetAllMovies();

            //iterate through the lsit and add them to the listbox
            foreach (BusinessTier.Movie movie in MOVIES)
                this.listBox15.Items.Add(movie.MovieName);
            ////////////// END PUTTING THE MOVIES IN THE LIST BOX

        }


        // #7 when they type one from the list
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox14.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // retrieve the info on the Movie based on their SELECTED CHOICE
            var MOVIE = biztier.GetMovie(this.textBox7.Text); // get the movie object (to get id num)
            if (MOVIE == null)
            {  // if the entered moviename isnt found
                this.listBox14.Items.Add("Movie entered was not found");
                return;
            }
            int MOVIEID = MOVIE.MovieID; // get the id num
            var movieDetail = biztier.GetMovieDetail(MOVIEID); // get the details (to get reviews)
            IReadOnlyList<BusinessTier.Review> REVIEWS = movieDetail.Reviews; // put the reviews into a list

            // print the user's username
            this.listBox14.Items.Add(MOVIE.MovieName);
            this.listBox14.Items.Add(""); // a newline


            int one = 0, two = 0, three = 0, four = 0, five = 0;
            int count = 0;

            // add 1 to each counter as I iterate through the reviews
            foreach (var i in REVIEWS)
            {
                switch (i.Rating)
                {
                    
                    case 1:
                        one++;
                        count++;
                        break;
                    case 2:
                        two++;
                        count++;
                        break;

                    case 3:
                        three++;
                        count++;
                        break;

                    case 4:
                        four++;
                        count++;
                        break;

                    case 5:
                        five++;
                        count++;
                        break;

                    default:
                        break;
                }
            }

            // these are the strings for the reviews
            string ONE, TWO, THREE, FOUR, FIVE;

            FIVE = String.Format("5: {0}", five);
            FOUR = String.Format("4: {0}", four);
            THREE = String.Format("3: {0}", three);
            TWO = String.Format("2: {0}", two);
            ONE = String.Format("1: {0}", one);

            this.listBox14.Items.Add(FIVE);
            this.listBox14.Items.Add(FOUR);
            this.listBox14.Items.Add(THREE);
            this.listBox14.Items.Add(TWO);
            this.listBox14.Items.Add(ONE);
            this.listBox14.Items.Add(String.Format("Total: {0}", count));
        }

        // #7 when they select one from the list
        private void listBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox14.Items.Clear();
            this.textBox7.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database

            // grab the object on the selected movie (to get its id)
            var MOVIE = biztier.GetMovie(this.listBox15.Text);
            // now grab its userid
            int movieID = MOVIE.MovieID;
            // now use its ID to get their review info
            var movieDETAIL = biztier.GetMovieDetail(movieID);
            // now finally, put its review information in a LIST
            IReadOnlyList<BusinessTier.Review> REVIEWS = movieDETAIL.Reviews;

            // print the user's username
            this.listBox14.Items.Add(MOVIE.MovieName);
            this.listBox14.Items.Add(""); // a newline

            
            int one = 0, two = 0, three = 0, four = 0, five = 0;
            int count = 0;
            
            // add 1 to each counter as I iterate through the reviews
            foreach (var i in REVIEWS)
            {
                switch (i.Rating)
                {
                    case 1:
                        one++;
                        count++;
                        break;
                    case 2:
                        two++;
                        count++;
                        break;

                    case 3:
                        three++;
                        count++;
                        break;

                    case 4:
                        four++;
                        count++;
                        break;

                    case 5:
                        five++;
                        count++;
                        break;

                    default:
                        break;
                }
            }

            // these are the strings for the reviews
            string ONE, TWO, THREE, FOUR, FIVE;

            FIVE = String.Format("5: {0}", five);
            FOUR = String.Format("4: {0}", four);
            THREE= String.Format("3: {0}", three);
            TWO  = String.Format("2: {0}", two);
            ONE  = String.Format("1: {0}", one);

            this.listBox14.Items.Add(FIVE);
            this.listBox14.Items.Add(FOUR);
            this.listBox14.Items.Add(THREE);
            this.listBox14.Items.Add(TWO);
            this.listBox14.Items.Add(ONE);
            this.listBox14.Items.Add(String.Format("Total: {0}", count));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // CLEAR THE LISTBOX FOR THE NEW ENTRY
            this.listBox16.Items.Clear();

            string filename = this.textBox1.Text; // db name THEN set up membrane
            if (!fileExists(filename)) // checks if the database file is valid
                return;
            BusinessTier.Business biztier = new BusinessTier.Business(filename); // creates a new class/object (OOP) which understands movies and reviews, need to just give it the name of the database
            
            // grab the inputted amount
            string top = this.textBox8.Text;
            // convert it to an int
            int topCount = Convert.ToInt32(top);

            if (topCount < 0){
                this.listBox16.Items.Add("Please enter a positive integer");
                return;
            }

            // grab the list of top "TopCount" movies
            IReadOnlyList<BusinessTier.Movie> MOVIES = biztier.GetTopMoviesByAvgRating(topCount);
            if (MOVIES == null || MOVIES.Count() == 0){ // SHOULD NEVER HAPPEN JUST A DEBUGGING TEST
                this.listBox16.Items.Add("ENTER IN AN INTEGER > 0");
                return;// SHOULD NEVER HAPPEN JUST A DEBUGGING TEST

            }

            string output;
            double avgReview;
            foreach (var i in MOVIES){
                var MOVIE = biztier.GetMovieDetail(i.MovieID);
                avgReview = MOVIE.AvgRating;
                output = String.Format("{0}: {1}", i.MovieName, avgReview);
                this.listBox16.Items.Add(output);
            }

        }
    }


}
