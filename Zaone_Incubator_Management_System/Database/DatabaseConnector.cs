using Zaone_Incubator_Management_System.Model;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Xamarin.Essentials;

namespace Zaone_Incubator_Management_System.Database
{
    public class DatabaseConnector
    {
        FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-4fbd5-default-rtdb.firebaseio.com/");


        //Add farmer
        public async Task<bool> AddFarmer(string username, string password, string email)
        {
            var FarmerIdentity = await firebase.Child("Farmer").OrderByKey().OnceAsync<Farmer>();

            // Check if the email already exists
            if (FarmerIdentity.Any(f => f.Object.Email == email))
            {
                return true; // Email already exists
            }

            int newID;
            if (FarmerIdentity.Any())
            {
                int maxId = FarmerIdentity.Max(f => f.Object.FarmerId);
                newID = maxId + 1;
            }
            else
            {
                newID = 1; // Start with 1 if no data exists
            }

            await firebase
                 .Child("Farmer")
                 .PostAsync(new Farmer() { FarmerId = newID, Username = username, Password = password, Email = email });

            return false; // Farmer added successfully
        }



        //Add Incubator
        public async Task<bool> AddIncubator(string name, string capacity)
        {
            var IncubatorIdentity = await firebase.Child("Incubator").OrderByKey().OnceAsync<Incubator>();

            // Check if the name already exists
            if (IncubatorIdentity.Any(f => f.Object.Name == name))
            {
                return true; // name already exists
            }

            int newID1;
            if (IncubatorIdentity.Any())
            {
                int maxId = IncubatorIdentity.Max(f => f.Object.IncubatorID);
                newID1 = maxId + 1;
            }
            else
            {
                newID1 = 1; // Start with 1 if no data exists
            }

            await firebase
            .Child("Incubator")
              .PostAsync(new Incubator() { IncubatorID = newID1, Name = name, Capacity = capacity, Status = 0 });
            return false; // Incubator added successfully
        }


        //LoginFarmer
        public async Task<Farmer> Login(string username, string password)
        {
            var snapshot = await firebase
                .Child("Farmer")
                .OnceAsync<Farmer>();

            var farmer = snapshot
                .Select(snap => snap.Object)
                .FirstOrDefault(f => f.Username == username && f.Password == password);

            return farmer;
        }



        //getIncubatorswithzerostatus
        public async Task<List<string>> GetIncubatorNamesWithStatusAsync()
        {
            try
            {
                var data = await firebase
                    .Child("Incubator")
                    .OnceAsync<Incubator>();

                var filteredIncubators = data
                    .Where(item => item.Object.Status == 0)
                    .Select(item => item.Object.Name)
                    .ToList();

                return filteredIncubators;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }



        //selectIncubatorOnDropdown
        public async Task<(string Capacity, int IncubatorID)> GetIncubatorDataByColumn1Async(string selectedValue)
        {
            try
            {
                var data = await firebase
                    .Child("Incubator")
                    .OnceAsync<Incubator>();

                var selectedRow = data.FirstOrDefault(item => item.Object.Name == selectedValue);

                if (selectedRow != null)
                {
                    return (selectedRow.Object.Capacity, selectedRow.Object.IncubatorID);
                }
                else
                {
                    return (null, -1); // Return -1 for IncubatorID to indicate data not found
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }




        //Add EggBatch
        public async Task EggBatch(int number, DateTime startdate, DateTime hatchdate, int incubatorId)
        {
            int IncubationDays = 21;
            var batchIdentity = await firebase.Child("EggBatch").OrderByKey().OnceAsync<EggBatch>();
            int newID1;

            if (batchIdentity.Any())
            {
                int maxId = batchIdentity.Max(f => f.Object.EggBatchID);
                newID1 = maxId + 1;
            }
            else
            {
                newID1 = 1; // Start with 1 if no data exists
            }

            // Insert data into EggBatch table
            await firebase.Child("EggBatch").PostAsync(new EggBatch()
            {
                EggBatchID = newID1,
                NumberofEggs = number,
                IncubationDays = IncubationDays,
                StartDate = startdate,
                HatchDate = hatchdate,
                IncubatorID = incubatorId,
                Status = 0
            });

            // Update Incubator table
            var incubatorToUpdate = await firebase.Child("Incubator").OrderByKey().OnceAsync<Incubator>();
            if (incubatorToUpdate.Any())
            {
                var incubator = incubatorToUpdate.FirstOrDefault(inc => inc.Object.IncubatorID == incubatorId);
                if (incubator != null)
                {
                    incubator.Object.Status = 1;
                    await firebase.Child("Incubator").Child(incubator.Key).PutAsync(incubator.Object);
                }
            }
        }


        //getAllEggBatchwithZeroStatus
        public async Task<List<(int EggBatchID, int IncubatorID, DateTime StartDate, DateTime HatchDate, string IncubatorName)>> GetAllEggBatchDataAsync()
        {
            try
            {
                var eggBatchData = await firebase.Child("EggBatch").OnceAsync<EggBatch>();
                var incubatorData = await firebase.Child("Incubator").OnceAsync<Incubator>();

                var eggBatchList = eggBatchData
                    .Where(item => item.Object.Status == 0) // Filter by status == 0
                    .Select(item =>
                    {
                        var incubatorId = item.Object.IncubatorID;
                        var incubator = incubatorData.FirstOrDefault(inc => inc.Object.IncubatorID == incubatorId);
                        var incubatorName = incubator != null ? incubator.Object.Name : "N/A"; // Provide a default value if no match is found
                        return (item.Object.EggBatchID, item.Object.IncubatorID, item.Object.StartDate, item.Object.HatchDate, incubatorName);
                    })
                    .ToList();

                return eggBatchList;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }

        //getAllEggBatchwithOneStatus
        public async Task<List<(int EggBatchID, int IncubatorID, DateTime StartDate, DateTime HatchDate, string IncubatorName)>> GetAllEggBatchDataSatus1()
        {
            try
            {
                var eggBatchData = await firebase.Child("EggBatch").OnceAsync<EggBatch>();
                var incubatorData = await firebase.Child("Incubator").OnceAsync<Incubator>();

                var eggBatchList = eggBatchData
                    .Where(item => item.Object.Status == 1) // Filter by status == 0
                    .Select(item =>
                    {
                        var incubatorId = item.Object.IncubatorID;
                        var incubator = incubatorData.FirstOrDefault(inc => inc.Object.IncubatorID == incubatorId);
                        var incubatorName = incubator != null ? incubator.Object.Name : "N/A"; // Provide a default value if no match is found
                        return (item.Object.EggBatchID, item.Object.IncubatorID, item.Object.StartDate, item.Object.HatchDate, incubatorName);
                    })
                    .ToList();

                return eggBatchList;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }


        //UpdateEggbatchandIncubatorStatusAndAddEggDetails
        public async Task UpdateEGGbatchAndIncubatorStatus(int incubatorId, int eggbatchId, int hatchedEggs, int unhatchedEggs)
        {
            try
            {
                var hatcheryIdentity = await firebase.Child("Hatchery").OrderByKey().OnceAsync<Hatchery>();
                int newID1;

                if (hatcheryIdentity.Any())
                {
                    int maxId = hatcheryIdentity.Max(f => f.Object.EggBatchID);
                    newID1 = maxId + 1;
                }
                else
                {
                    newID1 = 1; // Start with 1 if no data exists
                }


                // Update Incubator status
                var incubatorToUpdate = await firebase
                    .Child("Incubator")
                    .OrderByKey()
                    .OnceAsync<Incubator>();

                if (incubatorToUpdate.Any())
                {
                    var incubator = incubatorToUpdate.FirstOrDefault(inc => inc.Object.IncubatorID == incubatorId);
                    if (incubator != null)
                    {
                        incubator.Object.Status = 0; // Update the Status attribute
                        await firebase
                            .Child("Incubator")
                            .Child(incubator.Key)
                            .PutAsync(incubator.Object);
                    }
                }

                var eggBatchToUpdate = await firebase
                     .Child("EggBatch")
                     .OrderByKey()
                     .OnceAsync<EggBatch>();

                if (eggBatchToUpdate.Any())
                {
                    var eggBatch = eggBatchToUpdate.FirstOrDefault(egg => egg.Object.EggBatchID == eggbatchId);
                    if (eggBatch != null)
                    {
                        eggBatch.Object.Status = 1; // Update the Status attribute
                        await firebase
                            .Child("EggBatch")
                            .Child(eggBatch.Key)
                            .PutAsync(eggBatch.Object);

                        // Store the data in the Hatchery table
                        var hatcheryData = new Hatchery
                        {
                            HatcheryID = newID1,
                            IncubatorID = incubatorId,
                            EggBatchID = eggbatchId,
                            HatchedEggs = hatchedEggs,
                            UnhatchedEggs = unhatchedEggs
                        };

                        // Push the data to the Hatchery table
                        await firebase
                            .Child("Hatchery")
                            .PostAsync(hatcheryData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating Firebase: " + ex.Message);
            }
        }



        //GetEggCountForIncubatorfromEggBatch
        public async Task<int> GetNumberOfEggsAsync(int incubatorID)
        {
            try
            {
                var eggBatchQuery = await firebase
                    .Child("EggBatch")
                    .OrderBy("IncubatorID")
                    .EqualTo(incubatorID)
                    .OnceAsync<EggBatch>();



                int numberOfEggs = 0;
                foreach (var eggBatch in eggBatchQuery)
                {
         
                    numberOfEggs += eggBatch.Object.NumberofEggs;
                }


                return numberOfEggs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
        }








    }




}

