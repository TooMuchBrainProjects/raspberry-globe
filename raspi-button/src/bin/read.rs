use rppal::gpio::{Gpio, Level, Trigger};

fn main() {
    let gpio = Gpio::new().unwrap();
    let mut b1 = gpio.get(23).unwrap().into_input_pullup();
    b1.set_interrupt(Trigger::Both).unwrap();
    let mut l1 = gpio.get(24).unwrap().into_output_low();
    
    loop {
        let level = b1.poll_interrupt(false, None).unwrap();
        if let Some(Level::Low) = level {
            l1.set_high();
            println!("PUSHED");
        }
        else {
            l1.set_low();
            println!("RELEASED");
        }
    }
}
