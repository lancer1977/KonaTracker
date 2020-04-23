# Kona Tracker

## What is it?

A Xamarin Forms application written in C# that pulls a data set of date/county/state/cases/deaths and organizes the data in some different ways to provide at a glance info on trends throughout the United States.

## How It Works

1. Data is retrieved at launch from the New York Times github account.
2. Various options are presented on the leftside menu that provide paths into state details, or manual reload of the site.

# Why
I found myself reading a ton of websites that were causing me more anxiety when I just wanted to know how the daily trends were progressing. Initially I started with a console app and ended up with a mobile app instead due to the deploy anywhere nature of Xamarin (forms).

# Latest Build
*Android - https://install.appcenter.ms/users/lancer1977/apps/kona-viewer/distribution_groups/everyone
*iOS - contact me for an invite to the beta at lancer1977@gmail.com

# Data Sources
https://raw.githubusercontent.com/nytimes/covid-19-data/master/us-counties.csv
