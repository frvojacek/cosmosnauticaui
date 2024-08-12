import { FormEvent } from "react";
function App() {
    async function handleSubmit(event: FormEvent) {
        event.preventDefault();
        const target = event.target as HTMLFormElement;
        await fetch('https://localhost:32768/api/Document', {
            method: "POST",
            body: new FormData(target)
        });
    }

    return (
        <form onSubmit={handleSubmit}>
            <label htmlFor="file">File</label>
            <input id="file" name="file" type="file" />
            <button>Submit</button>
        </form>
    );
}

export default App;