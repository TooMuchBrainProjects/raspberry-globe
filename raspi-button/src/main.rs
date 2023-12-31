use rppal::gpio::{Gpio, Level, Trigger};
use reqwest::blocking::Client;
use serde::{Serialize, Deserialize};
use std::thread::sleep;
use std::time::Duration;

fn main() {
    let gpio = Gpio::new().unwrap();
    let mut b1 = gpio.get(23).unwrap().into_input_pullup();
    b1.set_interrupt(Trigger::FallingEdge).unwrap();
    let mut l1 = gpio.get(24).unwrap().into_output_low();

    let client = Client::new();  
    loop {
        let level = b1.poll_interrupt(true, None).unwrap();
        if let Some(Level::Low) = level {
            l1.set_high();
            
            match next(&client) {
                Ok(body) => {
                    println!("{:#?}", body);
                    sleep(Duration::from_secs(1));
                    l1.set_low();
                },
                Err(e) => {
                    println!("Error: {:#?}", e);
                    l1.set_low();
                    sleep(Duration::from_secs(1));
                    l1.set_high();
                    sleep(Duration::from_secs(1));
                    l1.set_low();
                }
            }
        }
    }
}

fn next(client: &Client) -> reqwest::Result<Function> {
    let resp = client.get("https://raspberry-globe-1.azurewebsites.net/api/next")
    .header("x-functions-key", "xi6eyMI1ddu6-RliD-tl7V4aty0EsXbg1E5eYOFwBo9vAzFuL-8RSQ==")
    .send()?;

    let mut func : Function = resp.json()?;
    func.key = String::from("***");
    return Ok(func);
}

#[derive(Serialize, Deserialize, Debug)]
struct Function {
    url: String,
    key: String
}
