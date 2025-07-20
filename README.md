# test

## config connetcion string 
    override ConnectionStrings.sql inside appsettings.Development.json
## run 
    update-database -context AppDbContext

 ## call api
    /seed-data
   
## login
   ### admin user
    user: admin  &  pass: 123456
   ### normal customer
    user: user1  &  pass: 123456

   ### /api/login
    get token and use for swagger auth (just put token without Bearer)