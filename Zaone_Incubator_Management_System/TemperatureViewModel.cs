using System.ComponentModel;
using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;
using Zaone_Incubator_Management_System.Model;
using Xamarin.Essentials;
using System.Linq;

public class TemperatureViewModel : INotifyPropertyChanged
{
    private double temperature;
    private double humidity;
    private int rotation;

    public double Temperature
    {
        get => temperature;
        set
        {
            if (temperature != value)
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }
    }

    public double Humidity
    {
        get => humidity;
        set
        {
            if (humidity != value)
            {
                humidity = value;
                OnPropertyChanged("Humidity");
            }
        }
    }



    private readonly FirebaseClient firebaseClient;

    public TemperatureViewModel()
    {
        // Initialize Firebase client
        firebaseClient = new FirebaseClient("https://xamarinfirebase-4fbd5-default-rtdb.firebaseio.com/");
    }

    public async Task SaveTemparatureHumidityRotation(int savedIncubatorID, int seconds)
    {
        // Check if the row with the key "yaccouv" already exists
        var existingSensor = await firebaseClient
            .Child("Sensor")
            .Child("yaccouv") // Specify the key "yaccouv"
            .OnceSingleAsync<SensorData>();

        if (existingSensor != null)
        {
            // If the row with key "yaccouv" exists, update it
            existingSensor.TemperatureValue = Temperature;
            existingSensor.HumidityValue = Humidity;
            existingSensor.EggRotationTimer = seconds; // Update EggRotationTimer

            await firebaseClient
                .Child("Sensor")
                .Child("yaccouv") // Specify the key "yaccouv"
                .PutAsync(existingSensor);
        }
        else
        {
            // If no row with key "yaccouv" exists, create a new one
            await firebaseClient
                .Child("Sensor")
                .Child("yaccouv") // Specify the key "yaccouv"
                .PutAsync(new SensorData
                {
                    TemperatureValue = Temperature,
                    HumidityValue = Humidity,
                    IncubatorID = savedIncubatorID,
                    EggRotationTimer = seconds // Add EggRotationTimer
                });
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
