using PaymentManager.Models; 
using PaymentManager.Services; 
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PaymentManager.ViewModels
{
    public class UserListViewModel
    {
        public ObservableCollection<User> Users { get; set; }
        public ICommand LoadUsersCommand { get; set; }

        // Public parameterless constructor  
        public UserListViewModel()
        {
            Users = new ObservableCollection<User>();
            LoadUsersCommand = new Command(LoadUsers);
        }

        private void LoadUsers()
        {
            // Example data loading logic  
            Users.Add(new User { Name = "John Doe" });
            Users.Add(new User { Name = "Jane Smith" });
        }
    }

    public class User
    {
        public string Name { get; set; }
    }
}
